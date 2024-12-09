using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs;

public class StudentDTO
{
    public string? Username { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string MobilePhoneNumber { get; set; }
    public string Gender { get; set; }

    // Full name property for convenience
    public string FullName => $"{FirstName} {LastName}";
}
