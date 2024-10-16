using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.Models;

//New class and Inherit from IdentityDbContext
public class AppDbContext : IdentityDbContext<UserModel, RoleModel, Guid>
{
    //New Table in DB
    public DbSet<ProductModel> Products { get; set; }

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
            new RoleModel { Id = Guid.Parse("dfb04ae0-0184-41ce-ac5d-1bee1ade19b3"), Name = "Customer", NormalizedName = "CUSTOMER", Description = "Customer role" },
            new RoleModel { Id = Guid.Parse("d1b172ba-4d15-4505-8de0-b43588da3359"), Name = "Seller", NormalizedName = "SELLER", Description = "Seller role" }
        );
    }


}

