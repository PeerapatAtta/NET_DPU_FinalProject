using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.DTOs.Request;
using WebAPI.DTOs.Response;
using WebAPI.Models;

namespace WebAPI.Controllers;


[ApiController]
[Route("[controller]")]

public class ProductsController : ControllerBase
{
    //DI AppDbContext Service
    private readonly AppDbContext _appDbContext;

    public ProductsController(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    [HttpGet]
    public async Task<IActionResult> GetProducts()
    {
        var results = await _appDbContext.Products.Select(x => new ProductDTO
        {
            Id = x.Id,
            Name = x.Name,
            Price = x.Price
        }).ToListAsync();

        //Send response
        return Ok(results);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProduct(Guid id)
    {
        var curProduct = await _appDbContext.Products.FirstOrDefaultAsync(x => x.Id == id);

        if (curProduct == null)
        {
            return NotFound();
        }
        var result = new ProductDetailDTO
        {
            Id = curProduct.Id,
            Name = curProduct.Name,
            Price = curProduct.Price,
            Description = curProduct.Description
        };
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> PostProduct(CreateProductDTO request)
    {
        //Add data from request to database
        var newProduct = new ProductModel
        {
            Name = request.Name,
            Price = request.Price,
            Description = request.Description
        };

        //Update database
        _appDbContext.Products.Add(newProduct);
        await _appDbContext.SaveChangesAsync();

        //Send response
        var result = new ProductDetailDTO
        {
            Id = newProduct.Id,
            Name = newProduct.Name,
            Price = newProduct.Price,
            Description = newProduct.Description
        };
        return CreatedAtAction(nameof(GetProduct), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutProduct(Guid id, UpdateProductDTO request)
    {
        //Find product in database Table by id
        var curProduct = await _appDbContext.Products.FirstOrDefaultAsync(x => x.Id == id);
        if (curProduct == null)
        {
            return NotFound();
        }
        //Update product data in DB from request
        curProduct.Name = request.Name;
        curProduct.Price = request.Price;
        curProduct.Description = request.Description;
        //Update database
        _appDbContext.Products.Update(curProduct);
        await _appDbContext.SaveChangesAsync();
        return NoContent();     
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(Guid id)
    {
        //Find product in database Table by id
        var curProduct = await _appDbContext.Products.FirstOrDefaultAsync(x => x.Id == id);

        if (curProduct == null)
        {
            return NotFound();
        }
        //Delete product from database
        _appDbContext.Products.Remove(curProduct);
        //Upate database
        await _appDbContext.SaveChangesAsync();
        //Send response
        return NoContent();

    }

}

