//2. RoleModel.cs //
using System;
using Microsoft.AspNetCore.Identity;

namespace WebAPI.Models;

public class RoleModel : IdentityRole<Guid>
{
    public string? Description { get; set; }
}
