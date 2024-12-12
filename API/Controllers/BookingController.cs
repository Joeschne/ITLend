using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.DTOs;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BookingController : ControllerBase
{
    private readonly ITLendDBContext _context;

    public BookingController(ITLendDBContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Gets the list of all bookings.
    /// </summary>
    /// <returns>A list of booking DTOs.</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BookingDTO>>> GetBookings()
    {
        List<BookingDTO> bookings = await _context.Bookings
            .Select(booking => MapToDTO(booking))
            .ToListAsync();

        return Ok(bookings);
    }

    /// <summary>
    /// Gets a specific booking by ID.
    /// </summary>
    /// <param name="id">The ID of the booking to retrieve.</param>
    /// <returns>The booking DTO if found, or a NotFound response.</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<BookingDTO>> GetBooking(int id)
    {
        Booking? booking = await _context.Bookings.FindAsync(id);

        if (booking == null)
        {
            return NotFound();
        }

        return Ok(MapToDTO(booking));
    }

    /// <summary>
    /// Creates a new booking.
    /// </summary>
    /// <param name="bookingDTO">The booking DTO to create.</param>
    /// <returns>The created booking DTO.</returns>
    [HttpPost]
    public async Task<ActionResult<BookingDTO>> PostBooking(BookingDTO bookingDTO)
    {
        int? studentId = await GetStudentIdFromDatabase(bookingDTO.StudentUsername);
        if (studentId == null) return BadRequest($"Invalid username: '{bookingDTO.StudentUsername}' not found.");

        int? laptopId = await GetLaptopIdFromDatabase(bookingDTO.LaptopIdentificationNumber);
        if (laptopId == null) return BadRequest($"Invalid laptop: '{bookingDTO.LaptopIdentificationNumber}' not found.");

        int? teacherId = await GetLaptopIdFromDatabase(bookingDTO.TeacherEmail);
        if (teacherId == null) return BadRequest($"Invalid teacher: '{bookingDTO.TeacherEmail}' not found.");

        Booking booking = new()
        {
            Id = bookingDTO.Id,
            StudentId = studentId.Value,
            LaptopId = laptopId.Value,
            TeacherId = teacherId,
            Returned = bookingDTO.Returned,
            BookingDateTime = bookingDTO.BookingDateTime,
            PlannedReturn = bookingDTO.PlannedReturn,
            Comment = bookingDTO.Comment
        };
        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetBooking), new { id = booking.Id }, MapToDTO(booking));
    }

    /// <summary>
    /// Updates an existing booking.
    /// </summary>
    /// <param name="id">The ID of the booking to update.</param>
    /// <param name="bookingDTO">The updated booking DTO.</param>
    /// <returns>No content if successful, or an appropriate error response.</returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> PutBooking(int id, BookingDTO bookingDTO)
    {
        if (id != bookingDTO.Id)
        {
            return BadRequest("ID mismatch.");
        }

        int? studentId = await GetStudentIdFromDatabase(bookingDTO.StudentUsername);
        if (studentId == null) return BadRequest($"Invalid username: '{bookingDTO.StudentUsername}' not found.");

        int? laptopId = await GetLaptopIdFromDatabase(bookingDTO.LaptopIdentificationNumber);
        if (laptopId == null) return BadRequest($"Invalid laptop: '{bookingDTO.LaptopIdentificationNumber}' not found.");

        int? teacherId = await GetLaptopIdFromDatabase(bookingDTO.TeacherEmail);
        if (teacherId == null) return BadRequest($"Invalid teacher: '{bookingDTO.TeacherEmail}' not found.");

        Booking? booking = await _context.Bookings.FindAsync(id);
        if (booking == null)
        {
            return NotFound();
        }

        booking.StudentId = studentId.Value;
        booking.LaptopId = laptopId.Value;
        booking.TeacherId = teacherId;
        booking.Returned = bookingDTO.Returned;
        booking.BookingDateTime = bookingDTO.BookingDateTime;
        booking.PlannedReturn = bookingDTO.PlannedReturn;
        booking.Comment = bookingDTO.Comment;

        _context.Entry(booking).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!BookingExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBooking(int id)
    {
        Booking? booking = await _context.Bookings.FindAsync(id);
        if (booking == null)
        {
            return NotFound();
        }

        _context.Bookings.Remove(booking);
        await _context.SaveChangesAsync();

        return NoContent();
    }


    private bool BookingExists(int id)
    {
        return _context.Bookings.Any(e => e.Id == id);
    }

    private BookingDTO MapToDTO(Booking booking)
    {
        return new BookingDTO
        {
            Id = booking.Id,
            StudentUsername = booking.Student.Username,
            LaptopIdentificationNumber = booking.Laptop.IdentificationNumber,
            TeacherEmail = booking.Teacher.Email,
            Returned = booking.Returned,
            BookingDateTime = booking.BookingDateTime,
            PlannedReturn = booking.PlannedReturn,
            Comment = booking.Comment
        };
    }

    private async Task<int?> GetStudentIdFromDatabase(string studentUsername)
    {
        Student? student = await _context.Students
        .FirstOrDefaultAsync(s => s.Username.ToLower() == studentUsername);

        return student?.Id;
    }

    private async Task<int?> GetLaptopIdFromDatabase(string laptopIdentificationNumber)
    {
        Laptop? laptop = await _context.Laptops
        .FirstOrDefaultAsync(l => l.IdentificationNumber.ToLower() == laptopIdentificationNumber);

        return laptop?.Id;
    }

    private async Task<int?> GetTeacherIdFromDatabase(string teacherEmail)
    {
        Teacher? teacher = await _context.Teachers
        .FirstOrDefaultAsync(t => t.Email.ToLower() == teacherEmail);

        return teacher?.Id;
    }
}
