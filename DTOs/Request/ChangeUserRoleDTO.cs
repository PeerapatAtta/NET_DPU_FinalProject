using System;
using System.ComponentModel.DataAnnotations;

namespace WebAPI.DTOs.Request;

public class ChangeUserRoleDto
{
    [Required(ErrorMessage = "Role is required.")]
    public required string Role { get; set; }
}
