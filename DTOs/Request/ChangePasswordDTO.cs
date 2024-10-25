using System;
using System.ComponentModel.DataAnnotations;

namespace WebAPI.DTOs.Request;

public class ChangePasswordDto
{
        [Required(ErrorMessage = "Old password is required.")]
        public string? OldPassword { get; set; }

        [Required(ErrorMessage = "New password is required.")]
        public string? NewPassword { get; set; }

        [Required(ErrorMessage = "Confirm password is required.")]
        [Compare(nameof(NewPassword), ErrorMessage = "New password and confirm password is mismatched.")]
        public string? ConfirmPassword { get; set; }
}
