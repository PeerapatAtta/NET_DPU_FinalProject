using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Models;

namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]

public class ProductController : ControllerBase
{
    static List<ProductModel> products = new List<ProductModel>();

    [HttpGet]
    public IActionResult GetProducts()
    {
        return Ok(products);
    }

    [HttpGet("{id}")]
    public IActionResult GetProduct(Guid id)
    {
        var product = products.FirstOrDefault(x => x.Id == id);
        if (product == null)
        {
            return NotFound();
        }
        return Ok(product);
    }

    [HttpPost]
    public IActionResult PostProduct(string name, double price)
    {
        var newProduct = new ProductModel { Id = Guid.NewGuid(), Name = name, Price = price };
        products.Add(newProduct);
        return CreatedAtAction(nameof(GetProduct), new { id = newProduct.Id }, newProduct);
    }

     [HttpPut("{id}")]
    public IActionResult PutProduct(Guid id, string name, double price)
    {
        var product = products.FirstOrDefault(x => x.Id == id);
        if (product == null)
        {
            return NotFound();
        }
        product.Name = name;
        product.Price = price;
        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteProduct(Guid id)
    {
        var product = products.FirstOrDefault(x => x.Id == id);
        if (product == null)
        {
            return NotFound();
        }
        products.Remove(product);
        return NoContent();
    }

}

