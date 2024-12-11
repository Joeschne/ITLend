using System.ComponentModel.DataAnnotations;

namespace API.Models;

public class Booking
{
    public int Id { get; set; }

    [Required]
    public int StudentId { get; set; }
    public Student Student { get; set; }

    [Required]
    public int LaptopId { get; set; }
    public Laptop Laptop { get; set; }

    [MaxLength(50)]
    public string? TeacherUsername { get; set; }

    [Required]
    public bool Returned { get; set; }

    [Required]
    public DateTime BookingDateTime { get; set; }

    [Required]
    public DateTime PlannedReturn { get; set; }

    [MaxLength(1000)]
    public string? Comment { get; set; }
}

