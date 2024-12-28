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

