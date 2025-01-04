using Shared.DTOs;

namespace Frontend.Interfaces
{
    public interface IStudentDetailService
    {
        /// <summary>
        /// Fetches the booking history for a specific student by username.
        /// </summary>
        /// <param name="username">The username of the student.</param>
        /// <returns>A list of BookingResponseDTO objects representing the student's booking history.</returns>
        Task<List<BookingResponseDTO>> GetStudentBookingsAsync(string username);
    }
}

