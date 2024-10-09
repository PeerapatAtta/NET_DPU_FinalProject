using System;

namespace WebAPI.DTOs.Response;

public class ProductDTO
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public double Price { get; set; }
}