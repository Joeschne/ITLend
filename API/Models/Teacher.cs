using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace API.Models;

[Index(nameof(Email), IsUnique = true)]
public class Teacher
{
    public int Id { get; set; }
    [Required]
    [MaxLength(30)]
    public string Email { get; set; }

    public ICollection<Booking> Bookings { get; set; }
}
