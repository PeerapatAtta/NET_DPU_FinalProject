using System;

namespace WebAPI.DTOs.Request;

public class UpdateCategoryDTO
{
    public string? Name { get; set; }
    public string? Description { get; set; }
}
