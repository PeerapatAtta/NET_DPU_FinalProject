using System;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.Models;

public class AppDbContext : DbContext
{

    public DbSet<ProductModel> Products { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    
}

