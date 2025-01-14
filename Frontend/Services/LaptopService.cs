using System.Net.Http.Json;
using Shared.DTOs;
using Frontend.Interfaces;

namespace Frontend.Services
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Frontend.Interfaces.ILaptopService" />
    public class LaptopService : ILaptopService
    {
        /// <summary>
        /// The HTTP client
        /// </summary>
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="LaptopService"/> class.
        /// </summary>
        /// <param name="httpClient">The HTTP client.</param>
        public LaptopService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Fetches the list of all laptops.
        /// </summary>
        /// <returns>
        /// A list of LaptopDTO objects.
        /// </returns>
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

        /// <summary>
        /// Fetches a specific laptop by ID.
        /// </summary>
        /// <param name="id">The ID of the laptop to retrieve.</param>
        /// <returns>
        /// A LaptopDTO object if found.
        /// </returns>
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

        /// <summary>
        /// Creates a new laptop.
        /// </summary>
        /// <param name="laptop">The LaptopDTO object to create.</param>
        /// <returns>
        /// The created LaptopDTO object.
        /// </returns>
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

        /// <summary>
        /// Updates an existing laptop.
        /// </summary>
        /// <param name="id">The ID of the laptop to update.</param>
        /// <param name="laptop">The updated LaptopDTO object.</param>
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

        /// <summary>
        /// Deletes a laptop by ID.
        /// </summary>
        /// <param name="id">The ID of the laptop to delete.</param>
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
