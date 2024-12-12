using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace API.Models;

[Index(nameof(IdentificationNumber), IsUnique = true)]
public class Laptop
{
    public int Id { get; set; }

    [Required]
    public bool IsAvailable { get; set; }

    [Required]
    [MaxLength(100)]
    public string Model { get; set; }

    [Required]
    [MaxLength(50)]
    public string IdentificationNumber { get; set; }

    [MaxLength(1000)]
    public string? DamageDescription { get; set; }

    public ICollection<Booking> Bookings { get; set; }
}

