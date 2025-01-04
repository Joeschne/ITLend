using Shared.DTOs;

namespace Frontend.Interfaces;

public interface IStudentService
{
    /// <summary>
    /// Searches for students by username.
    /// </summary>
    /// <param name="username">The username to search for.</param>
    /// <returns>A list of matching StudentDTOs.</returns>
    Task<IEnumerable<StudentDTO>> SearchStudentsAsync(string username);

    /// <summary>
    /// Adds a new student.
    /// </summary>
    /// <param name="student">The StudentDTO to add.</param>
    /// <returns>The added StudentDTO.</returns>
    Task<StudentDTO> AddStudentAsync(StudentDTO student);
}
