using BeaversTests.Api.Shared;
using BeaversTests.Identity.Api.Dtos;
using BeaversTests.Identity.Api.Firebase;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BeaversTests.Identity.Api.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class AuthController(
    IFirebaseAuthService authService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> SignUp([FromBody] SignUpDto signUpDto)
    {
        var token = await authService.SignUp(signUpDto.Email, signUpDto.Password);
        
        if (token is null)
            return BadRequest();
        
        HttpContext.Session.SetString(Auth.AuthTokenSessionKey, token);
        
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        var token = await authService.Login(loginDto.Email, loginDto.Password);
        
        if (token is null)
            return BadRequest();
        
        HttpContext.Session.SetString(Auth.AuthTokenSessionKey, token);
        
        return Ok(token);
    }
    
    [HttpGet]
    [Authorize]
    public IActionResult Logout()
    {
        authService.SignOut();
        
        HttpContext.Session.Remove(Auth.AuthTokenSessionKey);
        
        return Ok();
    }

    [HttpGet]
    [Authorize]
    public IActionResult Secure()
    {
        return Ok("secure str");
    }
}