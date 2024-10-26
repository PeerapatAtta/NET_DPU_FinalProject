using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.DTOs.Request;
using WebAPI.DTOs.Response;
using WebAPI.Models;

namespace WebAPI.Controllers;

[Route("[controller]")]
[ApiController]
[Authorize]

public class CatalogController : ControllerBase
{
    // Field with dependency injection
    private readonly AppDbContext _appDbContext; //คือการเก็บค่าของ AppDbContext ไว้ในตัวแปร _appDbContext

    // Constructor with dependency injection
    public CatalogController(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext; //คือการกำหนดค่าของ appDbContext ให้กับ _appDbContext
    }

    // Endpoint to get all categories
    [HttpGet]
    public async Task<IActionResult> GetCategories()
    {
        var categories = await _appDbContext.Catalogs //คือการดึงข้อมูลจากตาราง Catalogs ทั้งหมด
            .Select(c => new CategoryDTO //คือการสร้าง object ของ CategoryDTO โดยมีค่าเท่ากับข้อมูลในตาราง Catalogs
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description
            }).ToListAsync();//คือการแปลงข้อมูลที่ได้จากการดึงข้อมูลจากตาราง Catalogs ทั้งหมดเป็น List

        return Ok(categories);//คือการส่งข้อมูลที่ได้กลับไป
    }

    // Endpoint to get category by ID
    [HttpGet("{id}")]
    public async Task<IActionResult> GetCategory(Guid id)
    {
        var category = await _appDbContext.Catalogs.FindAsync(id);// คือการดึงข้อมูลจากตาราง Catalogs โดยมีเงื่อนไขว่า Id ต้องเท่ากับ id ที่ส่งมา

        // ถ้าไม่มีข้อมูลจะส่งค่า NotFound กลับไป
        if (category == null)
            return NotFound();

        // ถ้ามีข้อมูลจะส่งข้อมูลกลับไป
        var result = new CategoryDetailDTO
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description
        };

        return Ok(result);//คือการส่งข้อมูลที่ได้กลับไป
    }

    // Endpoint to create a new category
    [HttpPost]
    public async Task<IActionResult> CreateCategory(CreateCategoryDTO request) // คือการสร้างข้อมูลใหม่ในตาราง Catalogs
    {
        var newCategory = new CatalogModel // คือการสร้าง object ของ CatalogModel โดยมีค่าเท่ากับข้อมูลที่ส่งมาจาก request
        {
            Name = request.Name,
            Description = request.Description
        };

        _appDbContext.Catalogs.Add(newCategory); // คือการเพิ่มข้อมูลใหม่ลงในตาราง Catalogs
        await _appDbContext.SaveChangesAsync(); // คือการบันทึกข้อมูลลงในฐานข้อมูล

        var result = new CategoryDetailDTO  // คือการสร้าง object ของ CategoryDetailDTO โดยมีค่าเท่ากับข้อมูลที่เพิ่มลงในตาราง Catalogs
        {
            Id = newCategory.Id,
            Name = newCategory.Name,
            Description = newCategory.Description
        };

        return CreatedAtAction(nameof(GetCategory), new { id = result.Id }, result); // คือการส่งข้อมูลที่ได้กลับไป
    }

    // Endpoint to update a category
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCategory(Guid id, UpdateCategoryDTO request)
    {
        var category = await _appDbContext.Catalogs.FindAsync(id); // คือการดึงข้อมูลจากตาราง Catalogs โดยมีเงื่อนไขว่า Id ต้องเท่ากับ id ที่ส่งมา

        // ถ้าไม่มีข้อมูลจะส่งค่า NotFound กลับไป
        if (category == null)
            return NotFound();

        category.Name = request.Name ?? category.Name; // คือการเปลี่ยนแปลงข้อมูลในตาราง Catalogs โดยมีเงื่อนไขว่า Name ต้องเท่ากับ request.Name ถ้าไม่เท่ากับ request.Name จะเป็นค่าเดิม
        category.Description = request.Description ?? category.Description; // คือการเปลี่ยนแปลงข้อมูลในตาราง Catalogs โดยมีเงื่อนไขว่า Description ต้องเท่ากับ request.Description ถ้าไม่เท่ากับ request.Description จะเป็นค่าเดิม         

        _appDbContext.Catalogs.Update(category); // คือการอัพเดทข้อมูลในตาราง Catalogs
        await _appDbContext.SaveChangesAsync(); // คือการบันทึกข้อมูลลงในฐานข้อมูล

        return NoContent();
    }

    // Endpoint to delete a category
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCategory(Guid id)
    {
        var category = await _appDbContext.Catalogs.FindAsync(id); // คือการดึงข้อมูลจากตาราง Catalogs โดยมีเงื่อนไขว่า Id ต้องเท่ากับ id ที่ส่งมา
        
        // ถ้าไม่มีข้อมูลจะส่งค่า NotFound กลับไป
        if (category == null)
            return NotFound();

        _appDbContext.Catalogs.Remove(category); // คือการลบข้อมูลในตาราง Catalogs
        await _appDbContext.SaveChangesAsync(); // คือการบันทึกข้อมูลลงในฐานข้อมูล

        return NoContent();
    }
}

