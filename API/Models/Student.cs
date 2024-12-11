using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;


namespace API.Models;

[Index(nameof(Username), IsUnique = true)]
public class Student
{
    public int Id { get; set; }
    [Required]
    [MaxLength(30)]
    public string Username { get; set; }

    [Required]
    [MaxLength(50)]
    public string FirstName { get; set; }

    [Required]
    [MaxLength(50)]
    public string LastName { get; set; }

    [Required]
    [EmailAddress]
    [MaxLength(320)]
    public string Email { get; set; }

    [Required]
    [Phone]
    [MaxLength(30)]
    public string MobilePhoneNumber { get; set; }

    [Required]
    [MaxLength(10)]
    public string Gender { get; set; }

    public ICollection<Booking> Bookings { get; set; }
}

