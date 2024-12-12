namespace Shared.DTOs;

using System.ComponentModel.DataAnnotations;

public class StudentDetailDTO
{
    public int Id { get; set; }

    public string Username { get; set; }

    public List<BookingDTO> Bookings { get; set; }
}

