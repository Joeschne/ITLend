using API.Controllers;
using API.Models;
using Microsoft.EntityFrameworkCore;
using Shared.DTOs;

namespace API.Services;

/// <summary>
/// Provides mapping utilities for transforming entities and DTOs within the application.
/// </summary>
public class MappingService
{
    /// <summary>
    /// Maps a Student entity to a StudentDTO.
    /// </summary>
    /// <param name="student">The student entity.</param>
    /// <returns>A StudentDTO containing the student's data.</returns>
    public static StudentDTO MapToStudentDTO(Student student)
    {
        return new StudentDTO
        {
            Id = student.Id,
            Username = student.Username
        };
    }

    /// <summary>
    /// Maps a Student entity to a StudentDetailDTO.
    /// </summary>
    /// <param name="student">The student entity.</param>
    /// <returns>A StudentDetailDTO containing detailed student data, including bookings.</returns>
    public static StudentDetailDTO MapToStudentDetailDTO(Student student)
    {
        return new StudentDetailDTO
        {
            Id = student.Id,
            Username = student.Username,
            Bookings = student.Bookings.Select(b => MapToBookingResponseDTO(b)).ToList()
        };
    }

    /// <summary>
    /// Maps a StudentDTO to a Student entity.
    /// </summary>
    /// <param name="studentDTO">The student DTO.</param>
    /// <returns>A Student entity created from the DTO.</returns>
    public static Student MapToStudentModel(StudentDTO studentDTO)
    {
        return new Student
        {
            Id = studentDTO.Id,
            Username = studentDTO.Username
        };
    }

    /// <summary>
    /// Maps a Laptop entity to a LaptopDTO.
    /// </summary>
    /// <param name="laptop">The laptop entity.</param>
    /// <returns>A LaptopDTO containing the laptop's data.</returns>
    public static LaptopDTO MapToLaptopDTO(Laptop laptop)
    {
        return new LaptopDTO
        {
            Id = laptop.Id,
            IsAvailable = laptop.IsAvailable,
            Model = laptop.Model,
            IdentificationNumber = laptop.IdentificationNumber,
            DamageDescription = laptop.DamageDescription
        };
    }

    /// <summary>
    /// Maps a LaptopDTO to a Laptop entity.
    /// </summary>
    /// <param name="laptopDTO">The laptop DTO.</param>
    /// <returns>A Laptop entity created from the DTO.</returns>
    public static Laptop MapToLaptopModel(LaptopDTO laptopDTO)
    {
        return new Laptop
        {
            Id = laptopDTO.Id,
            IsAvailable = laptopDTO.IsAvailable,
            Model = laptopDTO.Model,
            IdentificationNumber = laptopDTO.IdentificationNumber,
            DamageDescription = laptopDTO.DamageDescription
        };
    }

    /// <summary>
    /// Maps a Teacher entity to a TeacherDTO.
    /// </summary>
    /// <param name="teacher">The teacher entity.</param>
    /// <returns>A TeacherDTO containing the teacher's data.</returns>
    public static TeacherDTO MapToTeacherDTO(Teacher teacher)
    {
        return new TeacherDTO
        {
            Id = teacher.Id,
            Email = teacher.Email
        };
    }

    /// <summary>
    /// Maps a Booking entity to a BookingResponseDTO.
    /// </summary>
    /// <param name="booking">The booking entity.</param>
    /// <returns>A BookingResponseDTO containing the booking's data.</returns>
    public static BookingResponseDTO MapToBookingResponseDTO(Booking booking)
    {
        return new BookingResponseDTO
        {
            Id = booking.Id,
            StudentUsername = booking.Student.Username,
            Laptop = MapToLaptopDTO(booking.Laptop),
            TeacherEmail = booking.Teacher.Email,
            Returned = booking.Returned,
            BookingDateTime = booking.BookingDateTime,
            PlannedReturn = booking.PlannedReturn,
            Comment = booking.Comment
        };
    }

    /// <summary>
    /// Updates a Booking entity using data from a BookingRequestDTO.
    /// </summary>
    /// <param name="booking">The existing Booking entity to update, or null to create a new one.</param>
    /// <param name="bookingDTO">The booking request DTO with new data.</param>
    /// <param name="context">The database context to fetch related entities.</param>
    /// <returns>The updated or newly created Booking entity.</returns>
    /// <exception cref="ArgumentException">Thrown if any related entity cannot be found.</exception>
    public static async Task<Booking> MapToBookingModel(Booking booking, BookingRequestDTO bookingDTO, ITLendDBContext context)
    {
        int? studentId = await context.Students
            .Where(s => s.Username.ToLower() == bookingDTO.StudentUsername.ToLower())
            .Select(s => s.Id)
            .FirstOrDefaultAsync();

        if (studentId == null) throw new ArgumentException($"Invalid username: '{bookingDTO.StudentUsername}' not found.");

        int? laptopId = await context.Laptops
            .Where(l => l.IdentificationNumber.ToLower() == bookingDTO.LaptopIdentificationNumber.ToLower())
            .Select(l => l.Id)
            .FirstOrDefaultAsync();

        if (laptopId == null) throw new ArgumentException($"Invalid laptop: '{bookingDTO.LaptopIdentificationNumber}' not found.");

        int? teacherId = await context.Teachers
            .Where(t => t.Email.ToLower() == bookingDTO.TeacherEmail.ToLower())
            .Select(t => t.Id)
            .FirstOrDefaultAsync();

        if (teacherId == null) throw new ArgumentException($"Invalid teacher: '{bookingDTO.TeacherEmail}' not found.");

        booking.StudentId = studentId.Value;
        booking.LaptopId = laptopId.Value;
        booking.TeacherId = teacherId.Value;
        booking.Returned = bookingDTO.Returned;
        booking.BookingDateTime = bookingDTO.BookingDateTime;
        booking.PlannedReturn = bookingDTO.PlannedReturn;
        booking.Comment = bookingDTO.Comment;

        return booking;
    }
}
