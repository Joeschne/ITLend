using Shared.DTOs;

namespace Frontend.Interfaces;

/// <summary>
/// Defines the contract for booking-related operations.
/// </summary>
public interface IBookingService
{
    /// <summary>
    /// Retrieves all bookings.
    /// </summary>
    /// <returns>An enumerable collection of booking DTOs.</returns>
    Task<IEnumerable<BookingResponseDTO>> GetBookingsAsync();

    /// <summary>
    /// Retrieves bookings that have not been returned.
    /// </summary>
    /// <returns>An enumerable collection of booking DTOs that are not returned.</returns>
    Task<IEnumerable<BookingResponseDTO>> GetNotReturnedBookingsAsync();

    /// <summary>
    /// Retrieves bookings that have been returned.
    /// </summary>
    /// <returns>An enumerable collection of booking DTOs that are returned.</returns>
    Task<IEnumerable<BookingResponseDTO>> GetReturnedBookingsAsync();

    /// <summary>
    /// Retrieves a specific booking by its ID.
    /// </summary>
    /// <param name="id">The ID of the booking to retrieve.</param>
    /// <returns>A booking DTO representing the booking with the specified ID.</returns>
    Task<BookingResponseDTO> GetBookingAsync(int id);

    /// <summary>
    /// Retrieves all bookings associated with a specific student.
    /// </summary>
    /// <param name="username">The username of the student.</param>
    /// <returns>A StudentDetailDTO containing the student's username and their bookings.</returns>
    Task<StudentDetailDTO> GetStudentBookingsAsync(string username);

    /// <summary>
    /// Retrieves all bookings associated with a specific laptop.
    /// </summary>
    /// <param name="laptopId">The ID of the laptop.</param>
    /// <returns>An enumerable collection of booking DTOs associated with the specified laptop.</returns>
    Task<IEnumerable<BookingResponseDTO>> GetBookingsByLaptopAsync(int laptopId);

    /// <summary>
    /// Creates a new booking.
    /// </summary>
    /// <param name="booking">The booking DTO containing details of the booking to create.</param>
    /// <returns>The created booking DTO.</returns>
    Task<BookingResponseDTO> CreateBookingAsync(BookingRequestDTO booking);

    /// <summary>
    /// Updates an existing booking.
    /// </summary>
    /// <param name="id">The ID of the booking to update.</param>
    /// <param name="booking">The booking DTO containing the updated details of the booking.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateBookingAsync(int id, BookingRequestDTO booking);

    /// <summary>
    /// Deletes a specific booking by its ID.
    /// </summary>
    /// <param name="id">The ID of the booking to delete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteBookingAsync(int id);
}
