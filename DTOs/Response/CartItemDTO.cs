using System;

namespace WebAPI.DTOs.Response;

public class CartItemDTO
{
    public Guid ProductId { get; set; }
    public string? ProductName { get; set; }
    public double Price { get; set; }
    public int Quantity { get; set; }
    public double TotalPrice => Price * Quantity;
}
