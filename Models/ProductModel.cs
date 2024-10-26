// ProductModel.cs //
using System;

namespace WebAPI.Models;

public class ProductModel
{
    public Guid Id { get; set; }  // Primary Key
    public string? Name { get; set; }
    public double Price { get; set; }
    public string? Description { get; set; }

    // Foreign Key to Catalog
    public Guid? CatalogId { get; set; }  // Allow NULL for Foreign Key

    // Navigation Property
    public CatalogModel? Catalog { get; set; } //คือการสร้าง Navigation Property ขึ้นมาเพื่อเชื่อมโยงกับ CatalogModel โดยใช้ CatalogId ในการเชื่อมโยง
}
