using System.Net.Http.Json;
using Shared.DTOs;
using Frontend.Interfaces;

namespace Frontend.Services
{
    public class LaptopService : ILaptopService
    {
        private readonly HttpClient _httpClient;

        public LaptopService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<LaptopDTO>> GetLaptopsAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<LaptopDTO>>("api/laptop");
            }
            catch (HttpRequestException ex)
            {
                Console.Error.WriteLine($"Error fetching laptops: {ex.Message}");
                return new List<LaptopDTO>();
            }
        }

        public async Task<LaptopDTO> GetLaptopAsync(int id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<LaptopDTO>($"api/laptop/{id}");
            }
            catch (HttpRequestException ex)
            {
                Console.Error.WriteLine($"Error fetching laptop with ID {id}: {ex.Message}");
                return null;
            }
        }

        public async Task<LaptopDTO> CreateLaptopAsync(LaptopDTO laptop)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/laptop", laptop);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<LaptopDTO>();
            }
            catch (HttpRequestException ex)
            {
                Console.Error.WriteLine($"Error creating laptop: {ex.Message}");
                throw;
            }
        }

        public async Task UpdateLaptopAsync(int id, LaptopDTO laptop)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/laptop/{id}", laptop);
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                Console.Error.WriteLine($"Error updating laptop with ID {id}: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteLaptopAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/laptop/{id}");
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                Console.Error.WriteLine($"Error deleting laptop with ID {id}: {ex.Message}");
                throw;
            }
        }
    }
}
