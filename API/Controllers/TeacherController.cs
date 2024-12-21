using API.Models;
using API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TeacherController : ControllerBase
{
    private readonly ITLendDBContext _context;

    public TeacherController(ITLendDBContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Gets all teachers.
    /// </summary>
    /// <returns>A list of teachers.</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TeacherDTO>>> GetTeachers(
    [FromQuery] string? search = null)
    {
        IQueryable<Teacher> query = _context.Teachers.AsQueryable();

        if (!string.IsNullOrEmpty(search))
        {
            search = search.ToLower(); 
            query = query.Where(t => t.Email.ToLower().Contains(search)); 
        }

        List<TeacherDTO> teachers = await query
            .Select(teacher => MappingService.MapToTeacherDTO(teacher))
            .ToListAsync();

        return Ok(teachers);
}

    /// <summary>
    /// Gets a specific teacher by ID.
    /// </summary>
    /// <param name="id">The ID of the teacher to retrieve.</param>
    /// <returns>The teacher DTO if found, or NotFound if not.</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<TeacherDTO>> GetTeacher(int id)
    {
        var teacher = await _context.Teachers.FindAsync(id);

        if (teacher == null)
        {
            return NotFound();
        }

        return Ok(MappingService.MapToTeacherDTO(teacher));
    }

    /// <summary>
    /// Updates an existing teacher.
    /// </summary>
    /// <param name="id">The ID of the teacher to update.</param>
    /// <param name="teacherDTO">The updated teacher data.</param>
    /// <returns>No content if successful, or an appropriate error response.</returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> PutTeacher(int id, TeacherDTO teacherDTO)
    {
        if (id != teacherDTO.Id)
        {
            return BadRequest("ID mismatch.");
        }

        var teacher = await _context.Teachers.FindAsync(id);

        if (teacher == null)
        {
            return NotFound();
        }

        teacher.Email = teacherDTO.Email;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!TeacherExists(id))
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
    /// Creates a new teacher.
    /// </summary>
    /// <param name="teacherDTO">The teacher DTO to create.</param>
    /// <returns>The created teacher DTO.</returns>
    [HttpPost]
    public async Task<ActionResult<TeacherDTO>> PostTeacher(TeacherDTO teacherDTO)
    {
        var teacher = new Teacher
        {
            Email = teacherDTO.Email
        };

        _context.Teachers.Add(teacher);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetTeacher), new { id = teacher.Id }, MappingService.MapToTeacherDTO(teacher));
    }

    /// <summary>
    /// Deletes a teacher by ID.
    /// </summary>
    /// <param name="id">The ID of the teacher to delete.</param>
    /// <returns>No content if successful, or NotFound if the teacher doesn't exist.</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTeacher(int id)
    {
        var teacher = await _context.Teachers.FindAsync(id);

        if (teacher == null)
        {
            return NotFound();
        }

        _context.Teachers.Remove(teacher);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPost("send-email")]
    public async Task<IActionResult> SendEmail([FromBody] EmailRequest request, [FromServices] IEmailService emailService)
    {
        if (string.IsNullOrWhiteSpace(request.RecipientEmail) || string.IsNullOrWhiteSpace(request.Message))
        {
            return BadRequest("Recipient email and message cannot be empty.");
        }

        try
        {
            await emailService.SendEmailAsync(request.RecipientEmail, "Rückgabe des Notebooks", request.Message);
            return Ok("Email sent successfully.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error sending email: {ex.Message}");
        }
    }

    public class EmailRequest
    {
        public string RecipientEmail { get; set; }
        public string Message { get; set; }
    }


    private bool TeacherExists(int id)
    {
        return _context.Teachers.Any(e => e.Id == id);
    }
}
