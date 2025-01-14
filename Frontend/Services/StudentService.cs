using System.Net.Http.Json;
using Frontend.Interfaces;
using Shared.DTOs;

namespace Frontend.Services;

/// <summary>
/// 
/// </summary>
/// <seealso cref="Frontend.Interfaces.IStudentService" />
public class StudentService : IStudentService
{
    /// <summary>
    /// The HTTP client
    /// </summary>
    private readonly HttpClient _httpClient;
    /// <summary>
    /// The API URL
    /// </summary>
    private const string ApiUrl = "https://localhost:7120/api/Student";

    /// <summary>
    /// Initializes a new instance of the <see cref="StudentService"/> class.
    /// </summary>
    /// <param name="httpClient">The HTTP client.</param>
    public StudentService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary>
    /// Searches for students by username.
    /// </summary>
    /// <param name="username">The username to search for.</param>
    /// <returns>
    /// A list of matching StudentDTOs.
    /// </returns>
    public async Task<IEnumerable<StudentDTO>> SearchStudentsAsync(string username)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<StudentDTO>>($"{ApiUrl}/search?username={username}");
        }
        catch (HttpRequestException ex)
        {
            Console.Error.WriteLine($"Error searching students: {ex.Message}");
            return new List<StudentDTO>();
        }
    }

    /// <summary>
    /// Adds a new student.
    /// </summary>
    /// <param name="student">The StudentDTO to add.</param>
    /// <returns>
    /// The added StudentDTO.
    /// </returns>
    public async Task<StudentDTO> AddStudentAsync(StudentDTO student)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(ApiUrl, student);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<StudentDTO>();
        }
        catch (HttpRequestException ex)
        {
            Console.Error.WriteLine($"Error adding student: {ex.Message}");
            throw;
        }
    }
}
