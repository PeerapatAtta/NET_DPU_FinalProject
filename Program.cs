using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using WebAPI.Helpers;
using WebAPI.Models;

var builder = WebApplication.CreateBuilder(args);

// ----------------Service Section----------------//
var services = builder.Services;

// Add services to the container.
services.AddControllers();

//Add Service for DbContext
services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("sqlite"));
});

// add identity service and its db context for Authentication
services.AddIdentity<UserModel, RoleModel>(options =>
{
    //Email must don't be same to other account
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<AppDbContext>()//Add Entity Framework Store for Identity
.AddDefaultTokenProviders();//Add default token provider for password reset

//Add Service for CORS
services.AddCors(options =>
{
    options.AddPolicy("MyCors", config =>
    {
        config
        .WithOrigins(builder.Configuration.GetSection("AllowedOrigins")
        .Get<string[]>()!)
        .AllowAnyMethod()
        .AllowAnyHeader();
    });
});

// add authentication for jwt
var jwtSettings = builder.Configuration.GetSection("JwtSettings"); // get jwt settings from appsettings.json

// add jwt settings to services
services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; // authenticate is when user is authenticated
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; // challenge is when user is not authenticated
})
// add jwt bearer authentication
.AddJwtBearer(options =>
{
    // configure jwt bearer options for authentication middleware to validate token
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true, // check if token is expired
        ValidIssuer = jwtSettings["ValidIssuer"],
        ValidAudience = jwtSettings["ValidAudience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecurityKey"]!))
    };
});

//Add service for Helper class
services.AddScoped<TokenHelper>();//Add TokenHelper service
// add email sender service for sending email to user for confirmation, password reset, etc.
services.AddScoped<IEmailSender<UserModel>, FakeEmailSender>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();

// add swagger generator service to generate swagger document
services.AddSwaggerGen(options =>
{
    // add security definition for bearer token
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization", // must be " Authorization"
        In = ParameterLocation.Header, // must be "Header"
        Type = SecuritySchemeType.Http, // must be "Http"
        Scheme = "Bearer" // must be "Bearer"
    });

    // add security requirement for bearer token. Some endpoints require authentication
    options.OperationFilter<AuthorizeCheckOperationFilter>();
});


// ----------------App Section----------------//
// Middleware ถูกเพิ่มเข้าไปใน Pipeline ที่นี่

var app = builder.Build(); // Build the app

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Middleware สำหรับการบังคับให้ใช้ HTTPS
app.UseHttpsRedirection();

// add CORS middleware
app.UseCors("MyCors");

// add authentication middleware. Who are you?
app.UseAuthentication();

// add authorization middleware. What can you do?
app.UseAuthorization();

// Map endpoints สำหรับการทำงานของ routing
app.MapControllers();

app.Run();
