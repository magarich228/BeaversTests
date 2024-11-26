using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace BeaversTests.Client.Tests;

[TestFixture]
public class AuthTests
{
    [Test]
    [Category("Runtime")]
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
        
        Assert.That(loginResponse.IsSuccessStatusCode, Is.True);
        Assert.That(loginResponse.Headers.TryGetValues("Authorization", out var values), Is.True);

        var authHeaderValue = values?.FirstOrDefault();
        var authHeaderValueExists = !string.IsNullOrWhiteSpace(authHeaderValue);
        
        Assert.That(authHeaderValue, Is.Not.Null);
        Assert.That(authHeaderValue, Is.Not.Empty);

        if (authHeaderValueExists)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authHeaderValue.Substring(7));
            
            using var projectsResponse = client
                .GetAsync("/api/Projects/GetAll")
                .Result;
            
            Assert.That(projectsResponse.IsSuccessStatusCode, Is.True);

            var projects = projectsResponse.Content.ReadAsStringAsync().Result;
            
            Assert.That(projects, Is.Not.Null);
            Assert.That(projects, Is.Not.Empty);
        }
    }
}