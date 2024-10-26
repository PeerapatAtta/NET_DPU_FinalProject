using System;

namespace WebAPI.Models;

public class FavoriteModel
{
    public Guid Id { get; set; }  // Primary Key

    // Foreign Key to UserModel
    public Guid UserId { get; set; }
    public UserModel? User { get; set; }

    // Foreign Key to ProductModel
    public Guid ProductId { get; set; }
    public ProductModel? Product { get; set; }
}
