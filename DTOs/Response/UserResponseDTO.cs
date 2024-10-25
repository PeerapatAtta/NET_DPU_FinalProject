using System;

namespace WebAPI.DTOs.Response;

public class UserResponseDto
{
    public Guid Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? Role { get; set; }
    public bool IsSuspended { get; set; }  // เพิ่มฟิลด์แสดงสถานะการระงับบัญชี
}
