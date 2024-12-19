using System.ComponentModel.DataAnnotations;

namespace Shared.DTOs;

public class LaptopDTO
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Availability Status is required.")]
    public bool IsAvailable { get; set; }

    [Required(ErrorMessage = "Model is required.")]
    [StringLength(100, ErrorMessage = "Model cannot exceed 100 characters.")]
    public string Model { get; set; }

    [Required(ErrorMessage = "Serial Number is required.")]
    [StringLength(50, ErrorMessage = "Serial Number cannot exceed 50 characters.")]
    public string IdentificationNumber { get; set; }

    [StringLength(1000, ErrorMessage = "Damage Description cannot exceed 250 characters.")]
    public string? DamageDescription { get; set; }
}

