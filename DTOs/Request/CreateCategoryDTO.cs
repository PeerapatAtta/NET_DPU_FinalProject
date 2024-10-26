using System;
using System.ComponentModel.DataAnnotations;

namespace WebAPI.DTOs.Request;

public class CreateCategoryDTO
{
    [Required(ErrorMessage = "Name is required")]
    public required string Name { get; set; }

    public string? Description { get; set; }
}
