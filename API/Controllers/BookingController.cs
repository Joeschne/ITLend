using API.Models;
using API.Services;
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
    public async Task<ActionResult<IEnumerable<BookingResponseDTO>>> GetBookings()
    {
        List<BookingResponseDTO> bookings = await _context.Bookings
            .Include(b => b.Student)
            .Include(b => b.Laptop)
            .Include(b => b.Teacher)
            .OrderByDescending(b => b.BookingDateTime)
            .Select(booking => MappingService.MapToBookingResponseDTO(booking))
            .ToListAsync();

        return Ok(bookings);
    }

    /// <summary>
    /// Gets the list of bookings that are not returned yet.
    /// </summary>
    /// <returns>A list of booking DTOs.</returns>
    [HttpGet("notreturned")]
    public async Task<ActionResult<IEnumerable<BookingResponseDTO>>> GetNotReturnedBookings()
    {
        List<BookingResponseDTO> bookings = await _context.Bookings
            .Include(b => b.Student)
            .Include(b => b.Laptop)
            .Include(b => b.Teacher)
            .Where(b => !b.Returned)
            .OrderByDescending(b => b.BookingDateTime)
            .Select(booking => MappingService.MapToBookingResponseDTO(booking))
            .ToListAsync();

        return Ok(bookings);
    }

    /// <summary>
    /// Gets the list of bookings that are returned.
    /// </summary>
    /// <returns>A list of booking DTOs.</returns>
    [HttpGet("returned")]
    public async Task<ActionResult<IEnumerable<BookingResponseDTO>>> GetReturnedBookings()
    {
        List<BookingResponseDTO> bookings = await _context.Bookings
            .Include(b => b.Student)
            .Include(b => b.Laptop)
            .Include(b => b.Teacher)
            .Where(b => b.Returned)
            .OrderByDescending(b => b.BookingDateTime)
            .Select(booking => MappingService.MapToBookingResponseDTO(booking))
            .ToListAsync();

        return Ok(bookings);
    }

    /// <summary>
    /// Gets a specific booking by ID.
    /// </summary>
    /// <param name="id">The ID of the booking to retrieve.</param>
    /// <returns>The booking DTO if found, or a NotFound response.</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<BookingResponseDTO>> GetBooking(int id)
    {
        Booking? booking = await _context.Bookings
            .Include(b => b.Student)
            .Include(b => b.Laptop)
            .Include(b => b.Teacher)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (booking == null)
        {
            return NotFound();
        }

        return Ok(MappingService.MapToBookingResponseDTO(booking));
    }

    /// <summary>
    /// Creates a new booking.
    /// </summary>
    /// <param name="bookingDTO">The booking DTO to create.</param>
    /// <returns>The created booking DTO.</returns>
    [HttpPost]
    public async Task<ActionResult<BookingRequestDTO>> PostBooking(BookingRequestDTO bookingDTO)
    {
        Booking booking = new();
        try
        {
            booking = await MappingService.MapToBookingModel(booking, bookingDTO, _context);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }

        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();

        Booking? createdBooking = await _context.Bookings
            .Include(b => b.Student)
            .Include(b => b.Laptop)
            .Include(b => b.Teacher)
            .FirstOrDefaultAsync(b => b.Id == booking.Id);

        return CreatedAtAction(nameof(GetBooking), new { id = createdBooking.Id }, MappingService.MapToBookingResponseDTO(createdBooking));
    }

    /// <summary>
    /// Updates an existing booking.
    /// </summary>
    /// <param name="id">The ID of the booking to update.</param>
    /// <param name="bookingDTO">The updated booking DTO.</param>
    /// <returns>No content if successful, or an appropriate error response.</returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> PutBooking(int id, BookingRequestDTO bookingDTO)
    {
        if (id != bookingDTO.Id)
        {
            return BadRequest("ID mismatch.");
        }

        Booking? booking = await _context.Bookings.FindAsync(id);
        if (booking == null)
        {
            return NotFound();
        }

        try
        {
            await MappingService.MapToBookingModel(booking, bookingDTO, _context);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }

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

    /// <summary>
    /// Deletes a booking by ID.
    /// </summary>
    /// <param name="id">The ID of the booking to delete.</param>
    /// <returns>No content if successful, or a NotFound response.</returns>
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
}
