using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BeaversTests.Auth.Public;
using BeaversTests.Client.Http.Configuration;
using BeaversTests.Client.Http.Exceptions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;

namespace BeaversTests.Client.Http;

public class BeaversTestsBaseClient
{
    protected readonly HttpClient HttpClient;
    protected readonly BeaversTestsClientOptions Options;
    protected readonly ILogger Logger;
    protected readonly AsyncRetryPolicy<HttpResponseMessage> RetryPolicy;

    protected AuthTokens? AuthTokens;

    protected BeaversTestsBaseClient(
        HttpClient httpClient,
        BeaversTestsClientOptions options,
        ILogger logger)
    {
        HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        Options = options ?? throw new ArgumentNullException(nameof(options));
        Logger = logger;

        // Configure retry policy
        RetryPolicy = Policy
            .HandleResult<HttpResponseMessage>(r =>
                (int)r.StatusCode >= 500 || r.StatusCode == HttpStatusCode.RequestTimeout)
            .Or<HttpRequestException>()
            .WaitAndRetryAsync(
                Options.MaxRetryAttempts,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)) + Options.RetryDelay,
                onRetry: (outcome, timespan, retryCount, context) =>
                {
                    Logger?.LogWarning(
                        "Retry {RetryCount} after {Delay}ms for {OperationKey}. Status: {StatusCode}",
                        retryCount, 
                        timespan.TotalMilliseconds, 
                        context.OperationKey,
                        outcome.Result?.StatusCode.ToString() ?? outcome.Exception?.Message);
                });

        ConfigureHttpClient();
    }

    private void ConfigureHttpClient()
    {
        if (string.IsNullOrEmpty(Options.BaseUrl))
            throw new InvalidOperationException("BaseUrl is required in BeaversTestsClientOptions");

        HttpClient.BaseAddress = new Uri(Options.BaseUrl);
        HttpClient.Timeout = Options.Timeout;
        
        if (!string.IsNullOrEmpty(Options.UserAgent))
            HttpClient.DefaultRequestHeaders.Add("User-Agent", Options.UserAgent);
            
        if (!string.IsNullOrEmpty(Options.ClientVersion))
            HttpClient.DefaultRequestHeaders.Add("X-Client-Version", Options.ClientVersion);
            
        if (!string.IsNullOrEmpty(Options.ClientName))
            HttpClient.DefaultRequestHeaders.Add("X-Client-Name", Options.ClientName);

        if (!string.IsNullOrEmpty(Options.AccessToken))
        {
            SetAccessToken(Options.AccessToken);
        }
    }

    public void SetAccessToken(string? accessToken)
    {
        if (string.IsNullOrEmpty(accessToken))
            return;

        HttpClient.DefaultRequestHeaders.Remove("Authorization");
        HttpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
    }

    public void ClearAccessToken()
    {
        HttpClient.DefaultRequestHeaders.Remove("Authorization");
    }

    public void SetAuthTokens(AuthTokens tokens)
    {
        AuthTokens = tokens;
        if (tokens?.AccessToken != null)
        {
            SetAccessToken(tokens.AccessToken);
        }
    }

    protected async Task<BeaversApiResponse<T>> SendAsync<T>(
        HttpMethod method,
        string endpoint,
        object? data = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(endpoint))
            throw new ArgumentException("Endpoint cannot be null or empty", nameof(endpoint));

        try
        {
            using var request = CreateRequest(method, endpoint, data);

            var response = await RetryPolicy.ExecuteAsync(
                async (ctx) =>
                {
                    return await HttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
                        .ConfigureAwait(false);
                }, 
                new Context(endpoint ?? string.Empty));

            return await ProcessResponse<T>(response, endpoint).ConfigureAwait(false);
        }
        catch (HttpRequestException ex)
        {
            Logger?.LogError(ex, "Network error while calling {Endpoint}", endpoint);
            throw new BeaversTestsNetworkException($"Network error: {ex.Message}", ex);
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            Logger?.LogError(ex, "Request timeout while calling {Endpoint}", endpoint);
            throw new BeaversTestsClientException("Request timeout", HttpStatusCode.RequestTimeout);
        }
        catch (BeaversTestsClientException)
        {
            throw;
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Unexpected error while calling {Endpoint}", endpoint);
            throw new BeaversTestsClientException($"Unexpected error: {ex.Message}", HttpStatusCode.InternalServerError);
        }
    }

    private HttpRequestMessage CreateRequest(HttpMethod method, string endpoint, object? data)
    {
        var request = new HttpRequestMessage(method, endpoint);

        if (data != null && (method == HttpMethod.Post || method == HttpMethod.Put))
        {
            var json = JsonConvert.SerializeObject(data);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");
        }

        return request;
    }

    private async Task<BeaversApiResponse<T>> ProcessResponse<T>(HttpResponseMessage response, string? endpoint)
    {
        var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        Logger?.LogDebug("Response from {Endpoint}: {StatusCode} - {Content}",
            endpoint, response.StatusCode, content);

        if (response.IsSuccessStatusCode)
        {
            try
            {
                var data = JsonConvert.DeserializeObject<T>(content);
                return new BeaversApiResponse<T>
                {
                    Success = true,
                    Data = data,
                    StatusCode = (int)response.StatusCode
                };
            }
            catch (JsonException ex)
            {
                Logger?.LogError(ex, "Failed to deserialize response from {Endpoint}", endpoint);
                throw new BeaversTestsClientException(
                    "Failed to deserialize response",
                    HttpStatusCode.InternalServerError,
                    responseContent: content);
            }
        }

        // Handle specific error cases
        return await HandleErrorResponse<T>(response, content, endpoint).ConfigureAwait(false);
    }

    private Task<BeaversApiResponse<T>> HandleErrorResponse<T>(HttpResponseMessage response, string content, string? endpoint)
    {
        switch (response.StatusCode)
        {
            case HttpStatusCode.Unauthorized:
                Logger?.LogWarning("Unauthorized access to {Endpoint}", endpoint);
                throw new BeaversTestsAuthException("Authentication required", HttpStatusCode.Unauthorized);

            case HttpStatusCode.Forbidden:
                Logger?.LogWarning("Forbidden access to {Endpoint}", endpoint);
                throw new BeaversTestsClientException("Access forbidden", HttpStatusCode.Forbidden);

            case HttpStatusCode.NotFound:
                Logger?.LogWarning("Endpoint not found: {Endpoint}", endpoint);
                throw new BeaversTestsClientException("Endpoint not found", HttpStatusCode.NotFound);

            case HttpStatusCode.BadRequest:
                return Task.FromResult(new BeaversApiResponse<T>
                {
                    Success = false,
                    Error = "Bad request",
                    StatusCode = (int)response.StatusCode
                });

            case (HttpStatusCode)429: // TooManyRequests
                Logger?.LogWarning("Rate limit exceeded for {Endpoint}", endpoint);
                throw new BeaversTestsClientException("Rate limit exceeded", (HttpStatusCode)429);

            default:
                Logger?.LogError("Server error from {Endpoint}: {StatusCode} - {Content}",
                    endpoint, response.StatusCode, content);
                throw new BeaversTestsClientException(
                    $"Server error: {response.StatusCode}",
                    response.StatusCode,
                    responseContent: content);
        }
    }
}