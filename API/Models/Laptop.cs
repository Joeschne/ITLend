using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace API.Models;

[Index(nameof(SerialNumber), IsUnique = true)]
public class Laptop
{
    [Key]
    public int Id { get; set; }

    [Required]
    public bool AvailabilityStatus { get; set; }

    [Required]
    [MaxLength(100)]
    public string Model { get; set; }

    [Required]
    [MaxLength(50)]
    public string SerialNumber { get; set; }

    [MaxLength(1000)]
    public string? DamageDescription { get; set; }

    public ICollection<Booking> Bookings { get; set; }
}

