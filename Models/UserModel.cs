//2.UserModel.cs//
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

    /// <summary>
    /// Products that own by this user
    /// </summary>
    public ICollection<ProductModel> Products { get; set; } = new List<ProductModel>();// สร้างคอลเล็กชันของ ProductModel ที่เป็นของ User นี้ 
}

