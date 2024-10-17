using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebAPI.DTOs.Request;
using WebAPI.Models;

namespace WebAPI.Controllers;

[Route("[controller]")]
[ApiController]

public class AccountsController : ControllerBase
{
    //DI > Object and Constructor
    //Object 
    private readonly UserManager<UserModel> userManager;

    //Constructor
    public AccountsController(UserManager<UserModel> _userManager)
    {
        userManager = _userManager;
    }

    //Endpoint for Register
    [HttpPost("Register")]
    public async Task<IActionResult> RegisterUser(RegisterUserDTO request)
    {
        // Add request data to UserModel
        var newUser = new UserModel
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            UserName = request.Email
        };

        // create new user to database
        var result = await userManager.CreateAsync(newUser, request.Password!);
        
        // Check if user creation is failed
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(x => x.Description);
            return BadRequest(new { Errors = errors });
        }

        // assign user role
        try
        {
            await userManager.AddToRoleAsync(newUser, request.Role!);
        }
        catch (Exception ex)
        {
            await userManager.DeleteAsync(newUser); // If role assignment failed, delete user
            var errors = new[] { ex.Message };
            return BadRequest(new { Errors = errors });
        }

        return StatusCode(StatusCodes.Status201Created);
    }

}

