using System;
using System.ComponentModel.DataAnnotations;

namespace WebAPI.DTOs.Request;

public class UpdateProductDTO
{
    [Required(ErrorMessage = "Name is required")]
    public required string Name { get; set; }

    [Required(ErrorMessage = "Price is required")]
    public double Price { get; set; }

    public string? Description { get; set; }

}