using System.ComponentModel.DataAnnotations;

namespace WebAPI.DTOs.Request;

public class LoginUserDTO
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email.")] // check if email is valid
    public string? Email { get; set; }

    [Required(ErrorMessage = "Password is required.")]
    public string? Password { get; set; }
}
