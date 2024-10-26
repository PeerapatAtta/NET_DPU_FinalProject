//UserModel.cs//
using System;
using Microsoft.AspNetCore.Identity;

namespace WebAPI.Models;

public class UserModel : IdentityUser<Guid>
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
    public string FullName => $"{FirstName} {LastName}";
    public bool IsSuspended { get; set; } = false;  // เพิ่มฟิลด์สำหรับสถานะระงับบัญชี

    // Relationship: One User can have many Products
    public ICollection<ProductModel> Products { get; set; } = new List<ProductModel>();// สร้างคอลเล็กชันของ ProductModel ที่เป็นของ User นี้ 

    // Relationship: One User can have many Favorites
    public ICollection<FavoriteModel> Favorites { get; set; } = new List<FavoriteModel>(); // สร้างคอลเล็กชันของ FavoriteModel ที่เป็นของ User นี้

    // Relationship: One User can have many Carts
    public ICollection<CartModel> Carts { get; set; } = new List<CartModel>(); // สร้างคอลเล็กชันของ CartModel ที่เป็นของ User นี้
}

