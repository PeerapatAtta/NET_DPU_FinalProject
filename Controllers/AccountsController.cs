using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using WebAPI.DTOs.Request;
using WebAPI.DTOs.Response;
using WebAPI.Helpers;
using WebAPI.Models;

namespace WebAPI.Controllers;

[Route("[controller]")]
[ApiController]

public class AccountsController : ControllerBase
{
    // private fields for dependency injection 
    private readonly UserManager<UserModel> userManager;
    private readonly TokenHelper tokenHelper;
    private readonly IEmailSender<UserModel> emailSender;

    // constructor for dependency injection
    public AccountsController(UserManager<UserModel> _userManager, TokenHelper _tokenHelper, IEmailSender<UserModel> _emailSender)
    {
        // assign dependency injection to private fields
        userManager = _userManager;
        tokenHelper = _tokenHelper;
        emailSender = _emailSender;
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

    //Endpoint for Making access token and refress token
    [HttpPost("Token/Refresh")]
    public async Task<IActionResult> RefreshToken(RefreshTokenDTO request)
    {
        // get username from expired access token
        string? username;

        try
        {
            // get username from expired access token
            var claimsPrincipal = tokenHelper.GetClaimsPrincipalFromExpiredToken(request.AccessToken!);
            username = claimsPrincipal.FindFirstValue("preferred_username");
        }
        catch (Exception ex)
        {
            var errors = new[] { ex.Message };
            return Unauthorized(new { Errors = errors });
        }

        // get user in DB from username
        var user = await userManager.FindByNameAsync(username!);

        // check if user exists and refresh token is valid and not expired
        if (user is null || user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            // return 401 Unauthorized with error message
            var errors = new[] { "Invalid token." };
            // return 401 Unauthorized with error message
            return Unauthorized(new { Errors = errors });
        }

        // create new access token and refresh token but don't extend refresh token expiry time
        var token = await tokenHelper.CreateToken(user, false);

        // return new access token and refresh token
        return Ok(new TokenResultDTO { AccessToken = token.AccessToken, RefreshToken = token.RefreshToken });
    }

    //Endpoint for revoking refresh token to use when user logout
    [HttpPost("Token/Revoke")]
    [Authorize] // only authenticated/login users can access this endpoint
    public async Task<IActionResult> RevokeToken([FromBody] object request)
    {
        // get username from access token
        var username = User.FindFirstValue("preferred_username");

        // get user in DB from username
        var user = await userManager.FindByNameAsync(username!);

        // check if user exists
        if (user is null)
        {
            var errors = new[] { "Invalid revoke request." };
            return BadRequest(new { Errors = errors });
        }

        // revoke refresh token
        user.RefreshToken = null;
        user.RefreshTokenExpiryTime = null;

        // update user in DB
        await userManager.UpdateAsync(user);

        // return 204 No Content
        return NoContent();
    }

    //Endpoint for forgot password
    [HttpPost("ForgotPassword")]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordDTO request)
    {
        // find user by email
        var user = await userManager.FindByEmailAsync(request.Email!);

        // if user is found
        if (user is not null)
        {
            // generate reset code example: 123456 abgt
            var resetCode = await userManager.GeneratePasswordResetTokenAsync(user);
            // encode reset code to base64url (ส่งไปเลยไม่ได้บางตัวอักษร http ไม่รองรับ) example: 123456%20abgt
            resetCode = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(resetCode));

            // create query params for reset link with token and email 
            //example: https://localhost:4200/account/resetpassword?code=123456%20abgt&email=example@gmail.com
            var queryParams = new Dictionary<string, string?>{
                { "token", resetCode },
                { "email", request.Email },
            };
            var resetLink = QueryHelpers.AddQueryString(request.ClientURI!, queryParams);// add query params to client uri for reset link
            await emailSender.SendPasswordResetLinkAsync(user, request.Email!, resetLink); // send password reset link to user
        }

        // return 204 No Content both user is found or not
        return NoContent();
    }

    //Endpoint for reset password
    [HttpPost("ResetPassword")]
    public async Task<IActionResult> ResetPassword(ResetPasswordDTO request)
    {
        // find user by email
        var user = await userManager.FindByEmailAsync(request.Email!);

        // if user is not found return 400 Bad Request
        if (user is null)
        {
            var errors = new[] { "Invalid reset password request." };
            return BadRequest(new { Errors = errors });
        }

        // decode reset code from base64url to normal string
        var resetCode = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Token!));

        // reset password with reset code and new password 
        var result = await userManager.ResetPasswordAsync(user, resetCode, request.Password!);

        // if reset password failed return 400 Bad Request
        if (!result.Succeeded)
        {
            return BadRequest(new { Errors = result.Errors.Select(e => e.Description) });
        }

        // return 204 No Content if reset password success
        return NoContent();
    }

}

