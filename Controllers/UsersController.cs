using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using WebAPI.DTOs.Request;
using WebAPI.DTOs.Response;
using WebAPI.Helpers;
using WebAPI.Models;

namespace WebAPI.Controllers;

[Route("[controller]")]
[ApiController]

public class UsersController : ControllerBase
{
    //Field with Dependency injection  
    private readonly UserManager<UserModel> _userManager; // To access the user manager
    private readonly RoleManager<RoleModel> _roleManager; // To access the role manager
    private readonly AppDbContext _appDbContext; // to access the database

    //Constructor with Dependency injection
    public UsersController(AppDbContext appDbContext, UserManager<UserModel> userManager, RoleManager<RoleModel> roleManager)
    {
        _appDbContext = appDbContext;
        _userManager = userManager;
        _roleManager = roleManager;
    }


    // Endpoint to get all users
    [HttpGet]
    [Authorize(Roles = "Seller")]
    public async Task<IActionResult>  GetAllUsers()
    {
        var users = await _userManager.Users
            .Select(user => new UserResponseDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Role = _userManager.GetRolesAsync(user).Result.FirstOrDefault() // ดึงชื่อบทบาท
            }).ToListAsync();

        return Ok(users);
    }

}
