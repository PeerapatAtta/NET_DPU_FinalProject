using System;
using System.ComponentModel.DataAnnotations;

namespace WebAPI.DTOs.Request;

public class ResetPasswordDTO
{
    [Required(ErrorMessage = "Password is required.")]
    public string? Password { get; set; }

    [Required(ErrorMessage = "Confirmation password is required."), Compare(nameof(Password), ErrorMessage = "Password and confirmation password is mismatched.")]
    public string? ConfirmPassword { get; set; }

    public string? Token { get; set; } // token from email link to reset password  

    public string? Email { get; set; } // email of the user who want to reset password  
}
