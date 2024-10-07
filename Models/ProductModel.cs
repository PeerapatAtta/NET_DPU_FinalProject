using System;

namespace WebAPI.Models;

public class ProductModel
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public double Price { get; set; }
}
