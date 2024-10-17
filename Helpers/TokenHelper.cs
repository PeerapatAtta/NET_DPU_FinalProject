using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using WebAPI.Models;

namespace WebAPI.Helpers;

public class TokenHelper
{   
    //DI > Object
    private readonly IConfigurationSection jwtSettings;
    private readonly IConfigurationSection refreshTokenSettings;
    private readonly UserManager<UserModel> userManager;

    //DI > Constructor
    public TokenHelper(IConfiguration configuration, UserManager<UserModel> userManager)
    {
        // Get the JwtSettings and RefreshTokenSettings from appsettings.json
        this.jwtSettings = configuration.GetSection("JwtSettings");
        this.refreshTokenSettings = configuration.GetSection("RefreshTokenSettings");
        this.userManager = userManager;
    }

    // Method to Create JWT Token (Access Token)
    public async Task<string> CreateJwtToken(UserModel user)
    {
        var signingCredentials = CreateSigningCredentials();// Create signing credentials for the JWT token header
        var claims = await CreateClaims(user); // Create claims for the JWT token payload data
        var jwtSecurityToken = CreatetJwtSecurityToken(signingCredentials, claims); // Create JWT security token (JWT Verify signature)
        var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken); // Write the JWT token to a string
        return token; // Return the JWT token
    }

    // Method to Create Refresh Token
    public string CreateRefreshToken()
    {
        // Generate a random number and convert it to a base64 string to be used as a refresh token 
        var rendomNumber = new byte[Convert.ToInt32(refreshTokenSettings["TokenLength"])];
        // Generate a random number
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(rendomNumber); // Fill the rendomNumber array with random bytes
        }
        return Convert.ToBase64String(rendomNumber); // Convert the random number to a base64 string
    }

    // Method to Create Token for using in Login
    public async Task<(string AccessToken, string RefreshToken)> CreateToken(UserModel user, bool populateExp = true)
    {
        var accessToken = await CreateJwtToken(user);
        user.RefreshToken = CreateRefreshToken();

        if (populateExp)
        {
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(Convert.ToDouble(refreshTokenSettings["ExpiryInMinutes"]));
        }

        await userManager.UpdateAsync(user);// Add new refresh token to user DB

        return (accessToken, user.RefreshToken);
    }

    // Method to Create Signing Credentials for JWT Token Header
    private SigningCredentials CreateSigningCredentials()
    {
        var key = Encoding.UTF8.GetBytes(jwtSettings["SecurityKey"]!);// Get the security key from appsettings.json
        var secret = new SymmetricSecurityKey(key);// Create a new SymmetricSecurityKey object
        return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256); // Create a new SigningCredentials object
    }

    // Method to Create Claims for JWT Token Payload Data
    private async Task<List<Claim>> CreateClaims(UserModel user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),// Add user Id to the claims
            new Claim(ClaimTypes.Name, user.UserName!),// Add user name to the claims
            new Claim(ClaimTypes.GivenName, user.FirstName + " " + user.LastName),// Add user full name to the claims
        };

        // Get the roles of the user from the database
        var roles = await userManager.GetRolesAsync(user);
        // Add each role to the claims
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        return claims;
    }

    // Method to Create JWT Security Token ( JWT Verify signature)
    private JwtSecurityToken CreatetJwtSecurityToken(SigningCredentials signingCredentials, List<Claim> claims)
    {
        // Create a new JWT security token
        var token = new JwtSecurityToken(
            issuer: jwtSettings["ValidIssuer"],
            audience: jwtSettings["ValidAudience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["ExpiryInMinutes"])),
            signingCredentials: signingCredentials // Add the signing credentials to the token
        );
        return token;
    }

}
