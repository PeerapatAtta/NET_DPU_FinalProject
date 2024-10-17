using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebAPI.DTOs.Request;
using WebAPI.Helpers;
using WebAPI.Models;

namespace WebAPI.Controllers;

[Route("[controller]")]
[ApiController]

public class AccountsController : ControllerBase
{
    //DDI>Object 
    private readonly UserManager<UserModel> userManager;
    private readonly TokenHelper tokenHelper;

    //DI>Constructor
    public AccountsController(UserManager<UserModel> _userManager, TokenHelper _tokenHelper)
    {
        userManager = _userManager;
        tokenHelper = _tokenHelper;
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

    //Endpoint for Login
    [HttpPost("Login")]
    public async Task<IActionResult> LoginUser(LoginUserDTO request)
    {
        // find user email in DB
        var user = await userManager.FindByEmailAsync(request.Email!);
        // Console.WriteLine(user != null ? user.ToString() : "User is null");

        // check user email and password in DB
        if (user is null || !await userManager.CheckPasswordAsync(user, request.Password!))
        {
            var errors = new[] { "Invalid email or password." };
            return Unauthorized(new { Errors = errors });
        }

        // create new tokens for Login
        var token = await tokenHelper.CreateToken(user);

        // return access token and refresh token to client
        return Ok(new TokenResultDTO { AccessToken = token.AccessToken, RefreshToken = token.RefreshToken });
    }

}

