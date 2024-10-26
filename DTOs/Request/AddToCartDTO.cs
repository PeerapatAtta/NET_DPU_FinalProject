using System;

namespace WebAPI.DTOs.Request;

public class AddToCartDTO
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}
