//UsersController.cs//
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

[Route("[controller]")] // กำหนด route ของ controller นี้ว่าจะใช้ชื่อว่าอะไร
[ApiController] // กำหนดว่า controller นี้เป็น API Controller
[Authorize] // ต้อง login ก่อนถึงจะใช้งานได้

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
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _userManager.Users
            .Select(user => new UserResponseDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                IsSuspended = user.IsSuspended, // เพิ่มสถานะการระงับบัญชีใน response
                Role = _userManager.GetRolesAsync(user).Result.FirstOrDefault() // ดึงชื่อบทบาท
            }).ToListAsync();

        return Ok(users);
    }

    // Endpoint to get user profile
    [HttpGet("Profile/{id}")]
    public async Task<IActionResult> GetProfile(Guid id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());// ค้นหาผู้ใช้จาก ID

        if (user == null) return NotFound(new { message = "User not found" });

        var roles = await _userManager.GetRolesAsync(user);// ดึงบทบาทของผู้ใช้

        // สร้าง response สำหรับส่งกลับ
        var response = new UserResponseDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Role = roles.FirstOrDefault()
        };

        return Ok(response);
    }

    // Endpoint to update user profile
    // PUT: Users/Profile/{id}
    [HttpPut("Profile/{id}")]
    public async Task<IActionResult> UpdateProfile(Guid id, UpdateUserProfileDto request)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());// ค้นหาผู้ใช้จาก ID

        if (user == null) return NotFound(new { message = "User not found" });// ถ้าไม่พบผู้ใช้ ส่งกลับ 404

        user.FirstName = request.FirstName ?? user.FirstName;// อัพเดทข้อมูล ถ้ามีการส่งข้อมูลมา
        user.LastName = request.LastName ?? user.LastName;// อัพเดทข้อมูล ถ้ามีการส่งข้อมูลมา
        user.Email = request.Email ?? user.Email;// อัพเดทข้อมูล ถ้ามีการส่งข้อมูลมา

        var result = await _userManager.UpdateAsync(user);// อัพเดทข้อมูลผู้ใช้ในฐานข้อมูล

        if (!result.Succeeded) return BadRequest(result.Errors);// ถ้าไม่สำเร็จ ส่งกลับ 400

        return NoContent();
    }

    // Endpoint to delete user profile
    // DELETE: Users/Profile/{id}
    [HttpDelete("Profile/{id}")]
    public async Task<IActionResult> DeleteProfile(Guid id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());// ค้นหาผู้ใช้จาก ID
        if (user == null) return NotFound(new { message = "User not found" });// ถ้าไม่พบผู้ใช้ ส่งกลับ 404

        var result = await _userManager.DeleteAsync(user);// ลบผู้ใช้ ออกจากฐานข้อมูล
        if (!result.Succeeded) return BadRequest(result.Errors);// ถ้าไม่สำเร็จ ส่งกลับ 400

        return NoContent();// ส่งกลับ 204
    }

    // Endpoint to change user password
    // PUT: Users/ChangeRole/{id}
    [HttpPut("ChangeRole/{id}")]
    // [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ChangeUserRole(Guid id, ChangeUserRoleDto request)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());// ค้นหาผู้ใช้จาก ID

        if (user == null) return NotFound(new { message = "User not found" });// ถ้าไม่พบผู้ใช้ ส่งกลับ 404

        var currentRoles = await _userManager.GetRolesAsync(user);// ดึงบทบาทปัจจุบันของผู้ใช้
        await _userManager.RemoveFromRolesAsync(user, currentRoles); // ลบบทบาทปัจจุบันทั้งหมด

        var roleExists = await _roleManager.RoleExistsAsync(request.Role);// ตรวจสอบว่าบทบาทที่ต้องการเปลี่ยนเป็นมีอยู่ในระบบหรือไม่
        if (!roleExists) return BadRequest(new { message = "Role does not exist" });// ถ้าไม่มี ส่งกลับ 400

        var result = await _userManager.AddToRoleAsync(user, request.Role);// เพิ่มบทบาทใหม่ให้กับผู้ใช้
        if (!result.Succeeded) return BadRequest(result.Errors);// ถ้าไม่สำเร็จ ส่งกลับ 400

        return NoContent();// ส่งกลับ 204
    }

    // Endpoint to change user password
    [HttpPut("ChangePassword/{id}")]
    public async Task<IActionResult> ChangePassword(Guid id, ChangePasswordDto request)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());// ค้นหาผู้ใช้จาก ID

        if (user == null) return NotFound(new { message = "User not found" });// ถ้าไม่พบผู้ใช้ ส่งกลับ 404

        // ตรวจสอบรหัสผ่านเก่าด้วย CheckPasswordAsync เพื่อให้การตรวจสอบรหัสผ่านเป็นไปตาม Identity Framework
        var isOldPasswordCorrect = await _userManager.CheckPasswordAsync(user, request.OldPassword!);
        if (!isOldPasswordCorrect) return BadRequest(new { message = "Old password is incorrect" });// ถ้ารหัสผ่านเก่าไม่ถูกต้อง ส่งกลับ 400

        // เปลี่ยนรหัสผ่านด้วยฟังก์ชัน ChangePasswordAsync
        var result = await _userManager.ChangePasswordAsync(user, request.OldPassword!, request.NewPassword!);// เปลี่ยนรหัสผ่าน
        if (!result.Succeeded) return BadRequest(result.Errors);// ถ้าไม่สำเร็จ ส่งกลับ 400

        return NoContent();// ส่งกลับ 204
    }

    // Endpoint to suspend user account
    // PUT: Users/Suspend/{id}
    [HttpPut("Suspend/{id}")]
    public async Task<IActionResult> SuspendAccount(Guid id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());// ค้นหาผู้ใช้จาก ID
        if (user == null) return NotFound(new { message = "User not found" });// ถ้าไม่พบผู้ใช้ ส่งกลับ 404

        user.IsSuspended = true;// ตั้งค่า IsSuspended เป็น true
        var result = await _userManager.UpdateAsync(user);// อัพเดทข้อมูลผู้ใช้ในฐานข้อมูล
        if (!result.Succeeded) return BadRequest(result.Errors);// ถ้าไม่สำเร็จ ส่งกลับ 400

        return NoContent();// ส่งกลับ 204
    }

    // Endpoint to unsuspend user account
    // PUT: Users/Unsuspend/{id}
    [HttpPut("Unsuspend/{id}")]
    public async Task<IActionResult> UnsuspendAccount(Guid id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());// ค้นหาผู้ใช้จาก ID
        if (user == null) return NotFound(new { message = "User not found" });// ถ้าไม่พบผู้ใช้ ส่งกลับ 404

        user.IsSuspended = false;// ตั้งค่า IsSuspended เป็น false
        var result = await _userManager.UpdateAsync(user);// อัพเดทข้อมูลผู้ใช้ในฐานข้อมูล
        if (!result.Succeeded) return BadRequest(result.Errors);// ถ้าไม่สำเร็จ ส่งกลับ 400

        return NoContent();// ส่งกลับ 204
    }

    // Endpoint to search users
    // GET: api/Users/Search?query={query}
    [HttpGet("Search")]
    public async Task<IActionResult> SearchUsers([FromQuery] string query) // รับ query จาก query string ด้วย [FromQuery] attribute 
    {
        // ค้นหาผู้ใช้ที่มีชื่อ หรือ นามสกุล หรือ อีเมล์ ตรงกับ query
        var users = await _userManager.Users
            .Where(u => (u.FirstName + " " + u.LastName).Contains(query) || u.Email!.Contains(query))// ค้นหาผู้ใช้ที่มีชื่อ หรือ นามสกุล หรือ อีเมล์ ตรงกับ query
            .Select(user => new UserResponseDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Role = _userManager.GetRolesAsync(user).Result.FirstOrDefault(),
                IsSuspended = user.IsSuspended  // เพิ่มสถานะการระงับบัญชีใน response
            })
            .ToListAsync();// แปลงผลลัพธ์เป็น List

        return Ok(users);// ส่งกลับผลลัพธ์ 200 OK พร้อมกับข้อมูลผู้ใช้
    }

}
