using System;
using System.Collections.Generic;

namespace WebAPI.Models;

public class CatalogModel
{
    public Guid Id { get; set; }  // Primary Key
    public string? Name { get; set; }
    public string? Description { get; set; }

    // Relationship: One Catalog can have many Products
    // สร้าง List ของ ProductModel ขึ้นมาเพื่อเก็บข้อมูลของ ProductModel ที่เกี่ยวข้องกับ CatalogModel นี้
    public ICollection<ProductModel> Products { get; set; } = new List<ProductModel>();
}
