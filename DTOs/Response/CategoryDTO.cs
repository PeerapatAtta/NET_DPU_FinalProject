using System;

namespace WebAPI.DTOs.Response;

public class CategoryDTO
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
}
