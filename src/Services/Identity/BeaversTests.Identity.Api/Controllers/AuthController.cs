using BeaversTests.Firebase.Auth;
using BeaversTests.Identity.Api.Dtos;
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
        
        HttpContext.Session.SetString("token", token);
        
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        var token = await authService.Login(loginDto.Email, loginDto.Password);
        
        if (token is null)
            return BadRequest();
        
        HttpContext.Session.SetString("token", token);
        
        return Ok();
    }
    
    [HttpGet]
    [Authorize]
    public IActionResult Logout()
    {
        authService.SignOut();
        
        HttpContext.Session.Remove("token");
        
        return Ok();
    }

    [HttpGet]
    [Authorize]
    public IActionResult Secure()
    {
        return Ok("secure str");
    }
}