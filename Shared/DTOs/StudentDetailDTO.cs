namespace Shared.DTOs;
public class StudentDetailDTO
{
    public int Id { get; set; }

    public string Username { get; set; }

    public List<BookingResponseDTO> Bookings { get; set; }
}

