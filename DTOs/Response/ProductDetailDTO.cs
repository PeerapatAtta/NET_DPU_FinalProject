// ProductDetailDTO.cs //
using System;

namespace WebAPI.DTOs.Response;

public class ProductDetailDTO
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public double Price { get; set; }
    public string? Description { get; set; }
    public string? CatalogName { get; set; }  // เพิ่มชื่อหมวดหมู่สินค้า

}
