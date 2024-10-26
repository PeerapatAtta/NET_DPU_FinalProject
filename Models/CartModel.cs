using System;

namespace WebAPI.Models;

public class CartModel
{
    public Guid Id { get; set; }  // Primary Key

    // Foreign Key to UserModel
    public Guid UserId { get; set; }
    public UserModel? User { get; set; }

    // Foreign Key to ProductModel
    public Guid ProductId { get; set; }
    public ProductModel? Product { get; set; }

    public int Quantity { get; set; }  // จำนวนสินค้าที่อยู่ในตะกร้า
}
