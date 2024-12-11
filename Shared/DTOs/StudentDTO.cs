using System.ComponentModel.DataAnnotations;

public class StudentDTO
{
    public int Id { get; set; }

    [Required]
    [MaxLength(30, ErrorMessage = "Username cannot exceed 30 characters.")]
    public string Username { get; set; }

    [Required(ErrorMessage = "First name is required.")]
    [MaxLength(50, ErrorMessage = "First name cannot exceed 50 characters.")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "Last name is required.")]
    [MaxLength(50, ErrorMessage = "Last name cannot exceed 50 characters.")]
    public string LastName { get; set; }

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Mobile phone number is required.")]
    [Phone(ErrorMessage = "Invalid phone number format.")]
    public string MobilePhoneNumber { get; set; }

    [Required(ErrorMessage = "Gender is required.")]
    [MaxLength(10, ErrorMessage = "Gender cannot exceed 10 characters.")]
    public string Gender { get; set; }

    public string FullName => $"{FirstName} {LastName}";
}
