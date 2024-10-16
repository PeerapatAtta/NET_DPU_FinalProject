using Microsoft.EntityFrameworkCore;
using WebAPI.Models;

var builder = WebApplication.CreateBuilder(args);

// ----------------Service Section----------------//
var services = builder.Services;
// Add services to the container.
services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

//Add Service for DbContext
services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("sqlite"));
});

// add identity service and its db context for Authentication
services.AddIdentity<UserModel, RoleModel>(options =>
{
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<AppDbContext>();

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


// ----------------App Section----------------//
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

// add CORS middleware
app.UseCors("MyCors");

// map enpoint for controller actions
app.MapControllers();

app.Run();
