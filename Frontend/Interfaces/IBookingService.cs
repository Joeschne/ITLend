using Shared.DTOs;

namespace Frontend.Interfaces;

public interface IBookingService
{
    Task<IEnumerable<BookingResponseDTO>> GetBookingsAsync();
    Task<IEnumerable<BookingResponseDTO>> GetNotReturnedBookingsAsync();
    Task<IEnumerable<BookingResponseDTO>> GetReturnedBookingsAsync();
    Task<BookingResponseDTO> GetBookingAsync(int id);
    Task<BookingResponseDTO> CreateBookingAsync(BookingRequestDTO booking);
    Task UpdateBookingAsync(int id, BookingRequestDTO booking);
    Task DeleteBookingAsync(int id);
}

