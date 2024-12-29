using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Frontend.Interfaces;
using Shared.DTOs;

namespace Frontend.Services;

public class BookingService : IBookingService
{
    private readonly HttpClient _httpClient;
    private const string ApiUrl = "https://localhost:7120/api/Booking";

    public BookingService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

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

    private void HandleError(HttpRequestException ex)
    {
        // Handle error appropriately, e.g., log to a service, show a notification, etc.
        Console.Error.WriteLine($"An error occurred: {ex.Message}");
    }
}

