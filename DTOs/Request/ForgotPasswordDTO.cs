using System;
using System.ComponentModel.DataAnnotations;

namespace WebAPI.DTOs.Request;

public class ForgotPasswordDTO
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Incorrect email.")] // check if email is valid
    public string? Email { get; set; }// email of the user

    [Required(ErrorMessage = "Client URI is required.")]
    public string? ClientURI { get; set; }// client uri to send email example: https://localhost:4200/account/resetpasswordS

}
