using Microsoft.AspNetCore.Mvc;

namespace Frontend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProxyController : ControllerBase
{
    private readonly HttpClient _httpClient;

    public ProxyController(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient();
    }

    [HttpGet("studentdata")]
    public async Task<IActionResult> GetStudentData([FromQuery] string username)
    {
        string apiUrl = $"https://europe-west6-gibz-app.cloudfunctions.net/Centerboard_GetStudentPersonData?username={Uri.EscapeDataString(username)}";
        Console.WriteLine($"Fetching data from: {apiUrl}");

        try
        {
            HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);
            Console.WriteLine($"Response Status Code: {response.StatusCode}");

            string content = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Response Content: {content}");

            response.EnsureSuccessStatusCode();

            if (content.StartsWith("<"))
            {
                Console.WriteLine("Received HTML response instead of JSON.");
                return StatusCode(500, "Received HTML response instead of JSON.");
            }

            return Content(content, "application/json");
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Error in proxy: {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
            }
            return StatusCode(500, $"Error fetching data: {ex.Message}");
        }
    }


}
