// ProductsController.cs //
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.DTOs.Request;
using WebAPI.DTOs.Response;
using WebAPI.Models;

namespace WebAPI.Controllers;

[ApiController]
//Base route for this controller
[Route("[controller]")]

public class ProductsController : ControllerBase
{
    //Field with dependency injection
    private readonly AppDbContext _appDbContext; //Store the value of AppDbContext in the variable _appDbContext

    //Constructor with dependency injection
    public ProductsController(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext; //Assign the value of appDbContext to _appDbContext
    }

    //Endpoint to get all products
    [HttpGet]
    public async Task<IActionResult> GetProducts()
    {
        var products = await _appDbContext.Products //คือการดึงข้อมูลจากตาราง Products ทั้งหมด
                    .Include(p => p.Catalog) //คือการดึงข้อมูลจากตาราง Catalog มาเชื่อมกับตาราง Products
                                             //คือการสร้าง object ของ ProductDTO โดยมีค่าเท่ากับข้อมูลในตาราง Products
                    .Select(p => new ProductDTO
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Price = p.Price,
                        CatalogName = p.Catalog != null ? p.Catalog.Name : "Unknown" //คือการเช็คว่า Catalog มีค่าเป็น null หรือไม่ ถ้าไม่ให้เอาชื่อ Catalog มาใส่ ถ้าใช่ให้ใส่ค่า "Unknown"
                    }).ToListAsync(); //คือการแปลงข้อมูลที่ได้จากการดึงข้อมูลจากตาราง Products ทั้งหมดเป็น List

        return Ok(products); //คือการส่งข้อมูลที่ได้กลับไป
    }

    //Endpoint to get product by ID
    [HttpGet("{id}")]
    public async Task<IActionResult> GetProduct(Guid id)
    {
        var product = await _appDbContext.Products //คือการดึงข้อมูลจากตาราง Products โดยมีเงื่อนไขว่า Id ต้องเท่ากับ id ที่ส่งมา
            .Include(p => p.Catalog) //คือการดึงข้อมูลจากตาราง Catalog มาเชื่อมกับตาราง Products
            .FirstOrDefaultAsync(p => p.Id == id); //คือการดึงข้อมูลจากตาราง Products โดยมีเงื่อนไขว่า Id ต้องเท่ากับ id ที่ส่งมา

        // ถ้าไม่มีข้อมูลจะส่งค่า NotFound กลับไป
        if (product == null)
            return NotFound();

        // ถ้ามีข้อมูลจะส่งข้อมูลกลับไป 
        var result = new ProductDetailDTO //คือการสร้าง object ของ ProductDetailDTO โดยมีค่าเท่ากับข้อมูลในตาราง Products
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            Description = product.Description,
            CatalogName = product.Catalog != null ? product.Catalog.Name : "Unknown" //คือการเช็คว่า Catalog มีค่าเป็น null หรือไม่ ถ้าไม่ให้เอาชื่อ Catalog มาใส่ ถ้าใช่ให้ใส่ค่า "Unknown"
        };

        return Ok(result); //คือการส่งข้อมูลที่ได้กลับไป
    }

    //Endpoint to create a new product
    [HttpPost]
    public async Task<IActionResult> PostProduct(CreateProductDTO request)
    {
        //Create new product in database Table
        var newProduct = new ProductModel
        {
            Name = request.Name,
            Price = request.Price,
            Description = request.Description,
            CatalogId = request.CatalogId
        };

        _appDbContext.Products.Add(newProduct); //Add new product to database
        await _appDbContext.SaveChangesAsync(); //Save changes to database

        //Create new object with product data to return in response body 
        var result = new ProductDetailDTO
        {
            Id = newProduct.Id,
            Name = newProduct.Name,
            Price = newProduct.Price,
            Description = newProduct.Description
        };

        return CreatedAtAction(nameof(GetProduct), new { id = result.Id }, result); //Return response with new product data
    }

    //Endpoint to update a product
    [HttpPut("{id}")]
    public async Task<IActionResult> PutProduct(Guid id, UpdateProductDTO request)
    {
        var product = await _appDbContext.Products.FindAsync(id); //Find product in database Table by id

        //If product not found, return NotFound response
        if (product == null)
            return NotFound();

        product.Name = request.Name ?? product.Name; //คือการเช็คว่า request.Name มีค่าเป็น null หรือไม่ ถ้าไม่ให้เอาค่า request.Name มาใส่ ถ้าใช่ให้เอาค่า product.Name มาใส่
        product.Price = request.Price ?? product.Price; //คือการเช็คว่า request.Price มีค่าเป็น null หรือไม่ ถ้าไม่ให้เอาค่า request.Price มาใส่ ถ้าใช่ให้เอาค่า product.Price มาใส่
        product.Description = request.Description ?? product.Description; //คือการเช็คว่า request.Description มีค่าเป็น null หรือไม่ ถ้าไม่ให้เอาค่า request.Description มาใส่ ถ้าใช่ให้เอาค่า product.Description มาใส่
        product.CatalogId = request.CatalogId ?? product.CatalogId; //คือการเช็คว่า request.CatalogId มีค่าเป็น null หรือไม่ ถ้าไม่ให้เอาค่า request.CatalogId มาใส่ ถ้าใช่ให้เอาค่า product.CatalogId มาใส่

        _appDbContext.Products.Update(product); //Update product in database
        await _appDbContext.SaveChangesAsync(); //Save changes to database

        return NoContent();
    }

    //Endpoint to delete a product
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(Guid id)
    {
        var product = await _appDbContext.Products.FindAsync(id); //Find product in database Table by id

        //If product not found, return NotFound response
        if (product == null)
            return NotFound();

        _appDbContext.Products.Remove(product); //Remove product from database
        await _appDbContext.SaveChangesAsync(); //Save changes to database

        return NoContent();
    }

    // New endpoint to search products by keyword
    [HttpGet("Search")]
    public async Task<IActionResult> SearchProducts([FromQuery] string query) // รับค่า query จาก query string
    {
        // ถ้า query มีค่าเป็น null หรือว่าง จะส่งค่า BadRequest กลับไป
        if (string.IsNullOrEmpty(query))
            return BadRequest(new { message = "Query parameter cannot be empty" });

        // to search products by keyword
        var products = await _appDbContext.Products 
            .Include(p => p.Catalog) // คือการดึงข้อมูลจากตาราง Catalog มาเชื่อมกับตาราง Products
            .Where(p => p.Name!.Contains(query) || p.Description!.Contains(query)) // คือการเช็คว่า Name หรือ Description มีค่าที่ตรงกับ query หรือไม่
            .Select(p => new ProductDTO // คือการสร้าง object ของ ProductDTO โดยมีค่าเท่ากับข้อมูลในตาราง Products
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                CatalogName = p.Catalog!.Name
            })
            .ToListAsync(); // คือการแปลงข้อมูลที่ได้จากการดึงข้อมูลจากตาราง Products ทั้งหมดเป็น List

        return Ok(products); // คือการส่งข้อมูลที่ได้กลับไป
    }

}

