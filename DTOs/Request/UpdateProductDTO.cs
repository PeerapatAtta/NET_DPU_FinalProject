// UpdateProductDTO.cs //
using System;
using System.ComponentModel.DataAnnotations;

namespace WebAPI.DTOs.Request;

public class UpdateProductDTO
{
    public string? Name { get; set; }

    public double Price { get; set; }
    
    public string? Description { get; set; }

}
