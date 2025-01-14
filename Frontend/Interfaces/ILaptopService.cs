using Shared.DTOs;

namespace Frontend.Interfaces
{
    public interface ILaptopService
    {
        /// <summary>
        /// Fetches the list of all laptops.
        /// </summary>
        /// <returns>A list of LaptopDTO objects.</returns>
        Task<List<LaptopDTO>> GetLaptopsAsync();

        /// <summary>
        /// Fetches a specific laptop by ID.
        /// </summary>
        /// <param name="id">The ID of the laptop to retrieve.</param>
        /// <returns>A LaptopDTO object if found.</returns>
        Task<LaptopDTO> GetLaptopAsync(int id);

        /// <summary>
        /// Creates a new laptop.
        /// </summary>
        /// <param name="laptop">The LaptopDTO object to create.</param>
        /// <returns>The created LaptopDTO object.</returns>
        Task<LaptopDTO> CreateLaptopAsync(LaptopDTO laptop);

        /// <summary>
        /// Updates an existing laptop.
        /// </summary>
        /// <param name="id">The ID of the laptop to update.</param>
        /// <param name="laptop">The updated LaptopDTO object.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task UpdateLaptopAsync(int id, LaptopDTO laptop);

        /// <summary>
        /// Deletes a laptop by ID.
        /// </summary>
        /// <param name="id">The ID of the laptop to delete.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task DeleteLaptopAsync(int id);
    }
}
