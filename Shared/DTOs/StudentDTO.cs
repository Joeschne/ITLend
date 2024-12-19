using System.ComponentModel.DataAnnotations;

public class StudentDTO
{
    public int Id { get; set; }

    [Required]
    [MaxLength(30, ErrorMessage = "Username cannot exceed 30 characters.")]
    public string Username { get; set; }

}
