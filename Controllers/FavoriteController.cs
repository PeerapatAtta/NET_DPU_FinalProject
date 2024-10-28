using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.DTOs.Request;
using WebAPI.DTOs.Response;
using WebAPI.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;


namespace WebAPI.Controllers;

[Route("[controller]")]
[ApiController]
[Authorize]

public class FavoriteController : ControllerBase
{
    // Field with dependency injection 
    private readonly AppDbContext _appDbContext; // Field to store injected context

    // Constructor with dependency injection
    public FavoriteController(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext; // Assign injected context to field
    }

    // Endpoint to get all favorites for the current user
    [HttpGet]
    public async Task<IActionResult> GetFavorites()
    {
        var userId = GetUserId(); // Method to retrieve current user ID
        var favorites = await _appDbContext.Favorites // Query to get favorites
            .Include(f => f.Product) // Include product details
            .Where(f => f.UserId == userId)     // Filter by current user
            .Select(f => new FavoriteProductDTO // Select DTO for response
            {
                ProductId = f.ProductId,
                ProductName = f.Product != null ? f.Product.Name : null,
                Price = f.Product != null ? f.Product.Price : null
            }).ToListAsync();   // Execute query and return as list

        return Ok(favorites); // Return list of favorites
    }

    // Endpoint to add a product to favorites
    [HttpPost]
    public async Task<IActionResult> AddFavorite(AddFavoriteDTO request)
    {
        var userId = GetUserId();   // Method to retrieve current user ID
        var favorite = new FavoriteModel    // Create new favorite model
        {
            UserId = userId,
            ProductId = request.ProductId
        };

        _appDbContext.Favorites.Add(favorite);  // Add favorite to context
        await _appDbContext.SaveChangesAsync(); // Save changes to database

        return Ok(new { message = "Product added to favorites" });  // Return success message
    }

    // Endpoint to remove a product from favorites
    [HttpDelete("{productId}")]
    public async Task<IActionResult> RemoveFavorite(Guid productId)
    {
        var userId = GetUserId();   // Method to retrieve current user ID
        var favorite = await _appDbContext.Favorites    // Query to find favorite
            .FirstOrDefaultAsync(f => f.UserId == userId && f.ProductId == productId);

        // If favorite not found, return 404 Not Found
        if (favorite == null)
            return NotFound();

        _appDbContext.Favorites.Remove(favorite);   // Remove favorite from context
        await _appDbContext.SaveChangesAsync();    // Save changes to database

        return NoContent();
    }

    // Endpoint to check if a product is in favorites
    [HttpGet("IsFavorite/{productId}")]
    public async Task<IActionResult> IsFavorite(Guid productId)
    {
        var userId = GetUserId();   // Method to retrieve current user ID

        // Check if favorite exists for current user and product
        var isFavorite = await _appDbContext.Favorites
            .AnyAsync(f => f.UserId == userId && f.ProductId == productId);

        return Ok(new IsFavoriteDTO { IsFavorite = isFavorite });   // Return result
    }

    // New endpoint to delete all favorites for the current user
    [HttpDelete("Clear")]
    public async Task<IActionResult> ClearFavorites()
    {
        var userId = GetUserId();  // Method to retrieve current user ID

        // Query to get all favorites for the current user
        var favorites = await _appDbContext.Favorites
            .Where(f => f.UserId == userId)
            .ToListAsync(); // Execute query and return as list

        _appDbContext.Favorites.RemoveRange(favorites); // Remove all favorites
        await _appDbContext.SaveChangesAsync(); // Save changes to database

        return Ok(new { message = "All favorites are remove" });
    }

    // New endpoint to get the count of favorites for the current user
    [HttpGet("Count")]
    public async Task<IActionResult> GetFavoriteCount()
    {
        var userId = GetUserId(); // ดึง ID ของผู้ใช้จาก claims
        var count = await _appDbContext.Favorites
            .CountAsync(f => f.UserId == userId); // นับจำนวนสินค้าที่ผู้ใช้นำไป favorite ไว้

        return Ok(new FavoriteCountDTO { Count = count }); // ส่งข้อมูล count กลับมาในรูปแบบ DTO
    }

    // Method to retrieve the current user's ID
    private Guid GetUserId()
    {
        // Retrieve user ID from claims
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get user ID from claims

        // If user ID is null or empty, throw an exception
        if (string.IsNullOrEmpty(userId))
        {
            throw new InvalidOperationException("User ID cannot be null or empty.");
        }

        return Guid.Parse(userId);  // Return user ID as Guid
    }
}



