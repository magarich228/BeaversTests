using System.ComponentModel.DataAnnotations;

namespace BeaversTests.Identity.Api.Dtos;

public class SignUpDto
{
    [Required, EmailAddress]
    public required string Email { get; set; }
    [Required]
    public required string Password { get; set; }
    [Required, Compare(nameof(Password), ErrorMessage = "The passwords didn't match.")]
    public required string ConfirmPassword { get; set; }
}