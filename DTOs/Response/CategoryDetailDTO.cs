using System;

namespace WebAPI.DTOs.Response;

public class CategoryDetailDTO
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
}
