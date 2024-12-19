using System.ComponentModel.DataAnnotations;

namespace Shared.DTOs;

public class BookingRequestDTO
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Student Username is required.")]
    [StringLength(50, ErrorMessage = "Student Username cannot exceed 50 characters.")]
    public string StudentUsername { get; set; }

    [Required(ErrorMessage = "Laptop IdentificationNumber is required.")]
    public string LaptopIdentificationNumber { get; set; }

    [StringLength(320, ErrorMessage = "Teacher email cannot exceed 50 characters.")]
    [EmailAddress]
    public string? TeacherEmail { get; set; } // Nullable for optional teacher assignments.

    [Required(ErrorMessage = "Returned status is required.")]
    public bool Returned { get; set; }

    [Required(ErrorMessage = "Booking DateTime is required.")]
    public DateTime BookingDateTime { get; set; }

    [Required(ErrorMessage = "Planned Return DateTime is required.")]
    public DateTime PlannedReturn { get; set; }

    [StringLength(1000, ErrorMessage ="Comment cannot exceed 1000 characters")]
    public string? Comment { get; set; }
}

