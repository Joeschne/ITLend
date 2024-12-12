namespace Shared.DTOs;

public class BookingResponseDTO
{
    public int Id { get; set; }

    public string StudentUsername { get; set; }

    public LaptopDTO Laptop { get; set; }

    public string? TeacherEmail { get; set; } // Nullable for optional teacher assignments.

    public bool Returned { get; set; }

    public DateTime BookingDateTime { get; set; }

    public DateTime PlannedReturn { get; set; }

    public string? Comment { get; set; }
}

