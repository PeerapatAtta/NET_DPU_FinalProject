//2. AppDbContext.cs //
using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.Models;

//New class and Inherit from IdentityDbContext
public class AppDbContext : IdentityDbContext<UserModel, RoleModel, Guid>
{
    //New Table in DB
    public DbSet<ProductModel> Products { get; set; } // เพิ่ม DbSet สำหรับ Products
    public DbSet<CatalogModel> Catalogs { get; set; } // เพิ่ม DbSet สำหรับ Catalogs

    //Constructor
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    //OnModelCreating Method to modify DB Table when migration
    protected override void OnModelCreating(ModelBuilder builder)
    {
        //Call base OnModelCreating Method
        base.OnModelCreating(builder);

        //Seed data(Add data when migration) for RoleModel
        builder.Entity<RoleModel>().HasData(
            new RoleModel
            {
                Id = Guid.Parse("dfb04ae0-0184-41ce-ac5d-1bee1ade19b3"),// กำหนดค่า Id ของ RoleModel ที่ต้องการให้เป็นค่าเดียวกับที่เราต้องการให้มีในฐานข้อมูล 
                Name = "Customer",
                NormalizedName = "CUSTOMER",
                Description = "Customer role"
            },
            new RoleModel
            {
                Id = Guid.Parse("d1b172ba-4d15-4505-8de0-b43588da3359"),
                Name = "Seller",
                NormalizedName = "SELLER",
                Description = "Seller role"
            },
            new RoleModel
            {
                Id = Guid.Parse("82b98abf-4f3e-4e4a-b5d8-bff84b3e48d2"),
                Name = "Admin",
                NormalizedName = "ADMIN",
                Description = "Admin role with full permissions"
            }
        );

        // ตั้งค่า default value เป็น false สำหรับฟิลด์ IsSuspended ใน UserModel
        builder.Entity<UserModel>()
            .Property(u => u.IsSuspended)
            .HasDefaultValue(false);  // ตั้งค่า default value เป็น false

        // กำหนดความสัมพันธ์ระหว่าง ProductModel และ CatalogModel
        builder.Entity<ProductModel>()
            .HasOne(p => p.Catalog) // กำหนดว่า ProductModel จะมี CatalogModel อยู่เพียงตัวเดียว
            .WithMany(c => c.Products) // กำหนดว่า CatalogModel จะมีหลาย ProductModel
            .HasForeignKey(p => p.CatalogId) // กำหนด Foreign Key ให้กับ CatalogId ใน ProductModel
            // .OnDelete(DeleteBehavior.Cascade);  // ถ้าลบ Catalog จะลบสินค้าทั้งหมดใน Catalog ด้วย
            .OnDelete(DeleteBehavior.SetNull);  // ตั้งค่าเป็น NULL ถ้า Catalog ถูกลบ

    }


}

