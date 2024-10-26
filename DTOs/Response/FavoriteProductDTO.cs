using System;

namespace WebAPI.DTOs.Response;

public class FavoriteProductDTO
{
    public Guid ProductId { get; set; }
    public string? ProductName { get; set; }
    public double? Price { get; set; }
}
