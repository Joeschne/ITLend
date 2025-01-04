using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Frontend.Interfaces;
using Shared.DTOs;

namespace Frontend.Services;

/// <summary>
/// 
/// </summary>
/// <seealso cref="Frontend.Interfaces.IBookingService" />
public class BookingService : IBookingService
{
    /// <summary>
    /// The HTTP client
    /// </summary>
    private readonly HttpClient _httpClient;
    /// <summary>
    /// The API URL
    /// </summary>
    private const string ApiUrl = "https://localhost:7120/api/Booking";

    /// <summary>
    /// Initializes a new instance of the <see cref="BookingService"/> class.
    /// </summary>
    /// <param name="httpClient">The HTTP client.</param>
    public BookingService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary>
    /// Retrieves all bookings.
    /// </summary>
    /// <returns>
    /// An enumerable collection of booking DTOs.
    /// </returns>
    public async Task<IEnumerable<BookingResponseDTO>> GetBookingsAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<BookingResponseDTO>>(ApiUrl);
        }
        catch (HttpRequestException ex)
        {
            HandleError(ex);
            throw;
        }
    }

    /// <summary>
    /// Retrieves bookings that have not been returned.
    /// </summary>
    /// <returns>
    /// An enumerable collection of booking DTOs that are not returned.
    /// </returns>
    public async Task<IEnumerable<BookingResponseDTO>> GetNotReturnedBookingsAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<BookingResponseDTO>>($"{ApiUrl}/notreturned");
        }
        catch (HttpRequestException ex)
        {
            HandleError(ex);
            throw;
        }
    }

    /// <summary>
    /// Retrieves bookings that have been returned.
    /// </summary>
    /// <returns>
    /// An enumerable collection of booking DTOs that are returned.
    /// </returns>
    public async Task<IEnumerable<BookingResponseDTO>> GetReturnedBookingsAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<BookingResponseDTO>>($"{ApiUrl}/returned");
        }
        catch (HttpRequestException ex)
        {
            HandleError(ex);
            throw;
        }
    }

    /// <summary>
    /// Retrieves all bookings associated with a specific student.
    /// </summary>
    /// <param name="username">The username of the student.</param>
    /// <returns>
    /// A StudentDetailDTO containing the student's username and their bookings.
    /// </returns>
    public async Task<StudentDetailDTO> GetStudentBookingsAsync(string username)
    {
        try
        {
            var bookings = await _httpClient.GetFromJsonAsync<List<BookingResponseDTO>>($"api/booking/by-student/{username}");
            return new StudentDetailDTO
            {
                Username = username,
                Bookings = bookings ?? new List<BookingResponseDTO>()
            };
        }
        catch (HttpRequestException ex)
        {
            Console.Error.WriteLine($"Error fetching bookings for student {username}: {ex.Message}");
            return new StudentDetailDTO
            {
                Username = username,
                Bookings = new List<BookingResponseDTO>()
            };
        }
    }

    /// <summary>
    /// Retrieves all bookings associated with a specific laptop.
    /// </summary>
    /// <param name="laptopId">The ID of the laptop.</param>
    /// <returns>
    /// An enumerable collection of booking DTOs associated with the specified laptop.
    /// </returns>
    public async Task<IEnumerable<BookingResponseDTO>> GetBookingsByLaptopAsync(int laptopId)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<BookingResponseDTO>>($"{ApiUrl}/by-laptop/{laptopId}");
        }
        catch (HttpRequestException ex)
        {
            Console.Error.WriteLine($"Error fetching bookings for laptop ID {laptopId}: {ex.Message}");
            return new List<BookingResponseDTO>();
        }
    }

    /// <summary>
    /// Retrieves a specific booking by its ID.
    /// </summary>
    /// <param name="id">The ID of the booking to retrieve.</param>
    /// <returns>
    /// A booking DTO representing the booking with the specified ID.
    /// </returns>
    public async Task<BookingResponseDTO> GetBookingAsync(int id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<BookingResponseDTO>($"{ApiUrl}/{id}");
        }
        catch (HttpRequestException ex)
        {
            HandleError(ex);
            throw;
        }
    }

    /// <summary>
    /// Creates a new booking.
    /// </summary>
    /// <param name="booking">The booking DTO containing details of the booking to create.</param>
    /// <returns>
    /// The created booking DTO.
    /// </returns>
    public async Task<BookingResponseDTO> CreateBookingAsync(BookingRequestDTO booking)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(ApiUrl, booking);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<BookingResponseDTO>();
        }
        catch (HttpRequestException ex)
        {
            HandleError(ex);
            throw;
        }
    }

    /// <summary>
    /// Updates an existing booking.
    /// </summary>
    /// <param name="id">The ID of the booking to update.</param>
    /// <param name="booking">The booking DTO containing the updated details of the booking.</param>
    public async Task UpdateBookingAsync(int id, BookingRequestDTO booking)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"{ApiUrl}/{id}", booking);
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException ex)
        {
            HandleError(ex);
            throw;
        }
    }

    /// <summary>
    /// Deletes a specific booking by its ID.
    /// </summary>
    /// <param name="id">The ID of the booking to delete.</param>
    public async Task DeleteBookingAsync(int id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"{ApiUrl}/{id}");
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException ex)
        {
            HandleError(ex);
            throw;
        }
    }

    /// <summary>
    /// Handles the error.
    /// </summary>
    /// <param name="ex">The ex.</param>
    private void HandleError(HttpRequestException ex)
    {
        // Handle error appropriately, e.g., log to a service, show a notification, etc.
        Console.Error.WriteLine($"An error occurred: {ex.Message}");
    }
}

