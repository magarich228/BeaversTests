using BeaversTests.Auth;
using BeaversTests.Auth.AspNetCore.Shared;
using BeaversTests.Auth.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BeaversTests.Manager.Api;
// TODO: users management controller

[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class AuthController(
    UserContext userContext,
    IAuthService authService, 
    ILogger<AuthController> logger) : ControllerBase
{
    /// <summary>
    /// Authenticates user with email and password
    /// </summary>
    /// <param name="request">Login credentials</param>
    /// <returns>Authentication tokens and user info</returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(AuthResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AuthResult>> Login([FromBody] LoginRequest request)
    {
        try
        {
            logger.LogInformation("Login request for email: {Email}", request.Email);

            var result = await authService.LoginAsync(request);

            if (!result.Success)
            {
                logger.LogWarning("Login failed for {Email}: {Error}", request.Email, result.Error);
                return BadRequest(result);
            }

            logger.LogInformation("Login successful for user: {Email} ({UserId})", request.Email, result.UserId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error during login for {Email}", request.Email);
            return StatusCode(StatusCodes.Status500InternalServerError, new AuthResult
            {
                Success = false,
                Error = "An unexpected error occurred during login"
            });
        }
    }

    /// <summary>
    /// Registers a new user account
    /// </summary>
    /// <param name="request">User registration data</param>
    /// <returns>Registration result with verification info</returns>
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(AuthResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AuthResult>> Register([FromBody] RegisterRequest request)
    {
        try
        {
            logger.LogInformation("Registration request for email: {Email}", request.Email);

            var result = await authService.RegisterAsync(request);

            if (!result.Success)
            {
                logger.LogWarning("Registration failed for {Email}: {Error}", request.Email, result.Error);
                return BadRequest(result);
            }

            logger.LogInformation("Registration successful for user: {Email} ({UserId})", request.Email, result.UserId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error during registration for {Email}", request.Email);
            return StatusCode(StatusCodes.Status500InternalServerError, new AuthResult
            {
                Success = false,
                Error = "An unexpected error occurred during registration"
            });
        }
    }

    /// <summary>
    /// Refreshes authentication tokens
    /// </summary>
    /// <param name="request">Refresh token data</param>
    /// <returns>New authentication tokens</returns>
    [HttpPost("refresh-token")]
    [ProducesResponseType(typeof(AuthResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(AuthResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AuthResult>> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        try
        {
            logger.LogInformation("Token refresh request");

            var result = await authService.RefreshTokenAsync(request);

            if (!result.Success)
            {
                logger.LogWarning("Token refresh failed: {Error}", result.Error);
                return BadRequest(result);
            }

            logger.LogInformation("Token refresh successful for user: {UserId}", result.UserId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error during token refresh");
            return StatusCode(StatusCodes.Status500InternalServerError, new AuthResult
            {
                Success = false,
                Error = "An unexpected error occurred during token refresh"
            });
        }
    }

    /// <summary>
    /// Logs out the current user and revokes tokens
    /// </summary>
    /// <returns>Logout result</returns>
    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(typeof(LogoutResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(LogoutResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<LogoutResult>> Logout()
    {
        try
        {
            var userId = userContext.GetCurrentUserId();

            logger.LogInformation("Logout request for user: {UserId}", userId);

            var result = await authService.LogoutAsync(userId);

            if (!result.Success)
            {
                logger.LogWarning("Logout failed for user {UserId}: {Error}", userId, result.Error);
                return BadRequest(result);
            }

            logger.LogInformation("Logout successful for user: {UserId}", userId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error during logout");
            return StatusCode(StatusCodes.Status500InternalServerError, new LogoutResult
            {
                Success = false,
                Error = "An unexpected error occurred during logout"
            });
        }
    }

    /// <summary>
    /// Initiates password reset process for the user
    /// </summary>
    /// <param name="request">Email for password reset</param>
    /// <returns>Operation result</returns>
    [HttpPost("reset-password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        try
        {
            logger.LogInformation("Password reset request for email: {Email}", request.Email);

            await authService.ResetPasswordAsync(request);

            // Always return success to prevent email enumeration
            return Ok(new { message = "If the email exists, a password reset link has been sent" });
        }
        catch (NotImplementedException)
        {
            logger.LogWarning("Reset password functionality not implemented");
            return StatusCode(StatusCodes.Status501NotImplemented, new { error = "Reset password functionality is not available" });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error during password reset for {Email}", request.Email);
            // Still return success to prevent email enumeration
            return Ok(new { message = "If the email exists, a password reset link has been sent" });
        }
    }

    /// <summary>
    /// Verifies user authentication status and get user info
    /// </summary>
    /// <returns>User info</returns>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(VerificationResult),StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public ActionResult GetCurrentUser()
    {
        var userInfo = userContext.GetCurrentUserInfo();

        logger.LogDebug("Authentication verification for user: {UserId} ({Email})", userInfo.UserId, userInfo.Email);

        return Ok(new VerificationResult()
        {
            IsAuthenticated = true,
            UserInfo = userInfo,
            Timestamp = DateTime.UtcNow
        });
    }
}