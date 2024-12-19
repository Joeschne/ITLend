using System.ComponentModel.DataAnnotations;

namespace Shared.DTOs;

public class TeacherDTO
{
    public int Id { get; set; }
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [StringLength(320, ErrorMessage = "Email cannot exceed 320 characters")]
    public string Email { get; set; }
}
