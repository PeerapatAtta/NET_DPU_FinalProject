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

public class CartController : ControllerBase
{
    // Injecting the AppDbContext into the controller
    private readonly AppDbContext _appDbContext;

    public CartController(AppDbContext appDbContext)
    {
        // Assigning the injected AppDbContext to a private field
        _appDbContext = appDbContext;
    }

    // Endpoint to get all items in the user's cart
    [HttpGet]
    public async Task<IActionResult> GetCartItems()
    {
        var userId = GetUserId();   // Retrieve user ID

        // Retrieve all items in the cart for the current user
        var cartItems = await _appDbContext.Carts
            .Include(c => c.Product)    // Include the product details
            .Where(c => c.UserId == userId) // Filter by user ID
            .Select(c => new CartItemDTO  // Project the results into a DTO   
            {
                ProductId = c.ProductId,
                ProductName = c.Product != null ? c.Product.Name : null,
                Price = c.Product != null ? c.Product.Price : 0,
                Quantity = c.Quantity
            }).ToListAsync();   // Execute the query and return the results as a list

        return Ok(cartItems);   // Return the list of cart items
    }

    // Endpoint to add an item to the cart
    [HttpPost]
    public async Task<IActionResult> AddToCart(AddToCartDTO request)
    {
        var userId = GetUserId();   // Retrieve user ID
        var cartItem = new CartModel    // Create a new cart item object with the request data
        {
            UserId = userId,    // Set the user ID
            ProductId = request.ProductId,  // Set the product ID
            Quantity = request.Quantity // Set the quantity
        };

        _appDbContext.Carts.Add(cartItem);  // Add the cart item to the database
        await _appDbContext.SaveChangesAsync();   // Save changes to the database

        return Ok(new { message = "Item added to cart" });  // Return a success message
    }

    // Endpoint to update quantity of an item in the cart
    [HttpPut("{productId}")]
    public async Task<IActionResult> UpdateCartItem(Guid productId, UpdateCartItemDTO request)
    {
        var userId = GetUserId();   // Retrieve user ID
        var cartItem = await _appDbContext.Carts    // Retrieve the cart item from the database
            .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId);  // Filter by user ID and product ID

        if (cartItem == null)   // If the cart item does not exist, return a 404 Not Found response
            return NotFound();

        cartItem.Quantity = request.Quantity;   // Update the quantity of the cart item
        await _appDbContext.SaveChangesAsync();  // Save changes to the database

        return NoContent();
    }

    // Endpoint to remove an item from the cart
    [HttpDelete("{productId}")]
    public async Task<IActionResult> RemoveCartItem(Guid productId)
    {
        var userId = GetUserId();   // Retrieve user ID
        var cartItem = await _appDbContext.Carts    // Retrieve the cart item from the database
            .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId);  // Filter by user ID and product ID

        if (cartItem == null)   // If the cart item does not exist, return a 404 Not Found response
            return NotFound();

        _appDbContext.Carts.Remove(cartItem);   // Remove the cart item from the database
        await _appDbContext.SaveChangesAsync(); // Save changes to the database

        return NoContent();
    }

    // Endpoint to clear all items in the cart
    [HttpDelete("Clear")]
    public async Task<IActionResult> ClearCart()
    {
        var userId = GetUserId();   // Retrieve user ID
        var cartItems = await _appDbContext.Carts   // Retrieve all cart items for the current user
            .Where(c => c.UserId == userId) // Filter by user ID
            .ToListAsync(); // Execute the query and return the results as a list     

        _appDbContext.Carts.RemoveRange(cartItems); // Remove all cart items from the database
        await _appDbContext.SaveChangesAsync(); // Save changes to the database

        return NoContent();
    }

    // Endpoint to get total amount of items in the cart
    [HttpGet("Total")]
    public async Task<IActionResult> GetCartTotal()
    {
        var userId = GetUserId();   // Retrieve user ID
        var totalAmount = await _appDbContext.Carts  // Calculate the total amount of items in the cart
            .Where(c => c.UserId == userId) // Filter by user ID
            .SumAsync(c => (c.Product != null ? c.Product.Price : 0) * c.Quantity); // Calculate the total amount     

        return Ok(new CartTotalDTO { TotalAmount = totalAmount });  // Return the total amount
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

