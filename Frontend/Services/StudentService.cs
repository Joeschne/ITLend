using System.Net.Http.Json;
using Frontend.Interfaces;
using Shared.DTOs;

namespace Frontend.Services;

public class StudentService : IStudentService
{
    private readonly HttpClient _httpClient;
    private const string ApiUrl = "https://localhost:7120/api/Student";

    public StudentService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

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
