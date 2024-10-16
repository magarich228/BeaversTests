namespace BeaversTests.TestsManager.Api.Middlewares;

public class ValidationErrorMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (FluentValidation.ValidationException exception)
        {
            context.Response.StatusCode = 400;
            context.Response.ContentType = "application/json";
            
            var errors = exception.Errors
                .Select(x => new
                {
                    x.PropertyName,
                    x.ErrorMessage
                })
                .ToList();
            
            await context.Response.WriteAsJsonAsync(errors);
        }
    }
}