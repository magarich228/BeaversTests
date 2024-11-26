using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace BeaversTests.Client.Tests;

[TestFixture]
public class AuthTests
{
    [Test]
    public void LoginAndGetProjectsTest()
    {
        using var client = new HttpClient();
        client.BaseAddress = new Uri("http://localhost:5008");

        var loginDto = new
        {
            Email = "akamagarich228@gmail.com",
            Password = "qwerty123"
        };
        
        using var loginResponse = client
            .PostAsync("/api/Auth/Login", JsonContent.Create(loginDto))
            .Result;
        
        Assert.True(loginResponse.IsSuccessStatusCode);
        Assert.True(loginResponse.Headers.TryGetValues("Authorization", out var values));

        var authHeaderValue = values?.FirstOrDefault();
        var authHeaderValueExists = !string.IsNullOrWhiteSpace(authHeaderValue);
        
        Assert.IsNotNull(authHeaderValue);
        Assert.IsNotEmpty(authHeaderValue);

        if (authHeaderValueExists)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authHeaderValue.Substring(7));
            
            using var projectsResponse = client
                .GetAsync("/api/Projects/GetAll")
                .Result;
            
            Assert.True(projectsResponse.IsSuccessStatusCode);

            var projects = projectsResponse.Content.ReadAsStringAsync().Result;
            
            Assert.IsNotNull(projects);
            Assert.IsNotEmpty(projects);
        }
    }
}