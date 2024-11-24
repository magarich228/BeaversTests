using BeaversTests.Firebase.Auth;
using Firebase.Auth;
using Firebase.Auth.Providers;
using FirebaseAdmin;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

services.AddSession();
services.AddDistributedMemoryCache();

Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS",
    "beaverstests-firebase-adminsdk-kp03n-8921020f2f.json");
services.AddSingleton(FirebaseApp.Create());

services.AddAuth();

services.AddControllers();

services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseSession();
app.MapControllers();

app.Use(async (context, next) =>
{
    var token = context.Session.GetString("token");
    
    if (!string.IsNullOrEmpty(token))
    {
        context.Request.Headers.Append("Authorization", "Bearer " + token);
    }

    await next();
});

app.UseAuth();

await app.RunAsync();

public static class Auth
{
    public static IServiceCollection AddAuth(this IServiceCollection services)
    {
        var firebaseProjectName = "beaverstests";
        services.AddSingleton(new FirebaseAuthClient(new FirebaseAuthConfig
        {
            ApiKey = "AIzaSyDh58sllS39Z6RSE-LTheJ_Adgbrl2Ot1c",
            AuthDomain = $"{firebaseProjectName}.firebaseapp.com",
            Providers = new FirebaseAuthProvider[]
            {
                new EmailProvider(),
                new GoogleProvider()
            }
        }));
        services.AddSingleton<IFirebaseAuthService, FirebaseAuthService>(); 
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = $"https://securetoken.google.com/{firebaseProjectName}";
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = $"https://securetoken.google.com/{firebaseProjectName}",
                    ValidateAudience = true,
                    ValidAudience = firebaseProjectName,
                    ValidateLifetime = true
                };
            });
        
        return services;
    }

    public static IApplicationBuilder UseAuth(this IApplicationBuilder app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
        
        return app;
    }
}