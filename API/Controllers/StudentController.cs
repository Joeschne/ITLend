using API.Models;
using API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.DTOs;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StudentController : ControllerBase
{
    private readonly ITLendDBContext _context;

    public StudentController(ITLendDBContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Gets the list of all students with optional search functionality.
    /// </summary>
    /// <param name="search">Optional search term to filter students by Username.</param>
    /// <returns>A list of student DTOs.</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<StudentDTO>>> GetStudents(
        [FromQuery] string? search = null)
    {
        IQueryable<Student> query = _context.Students.AsQueryable();

        if (!string.IsNullOrEmpty(search))
        {
            search = search.ToLower(); 
            query = query.Where(s => s.Username.ToLower().Contains(search)); 
        }

        List<StudentDTO> students = await query
            .Select(student => MappingService.MapToStudentDTO(student))
            .ToListAsync();

        return Ok(students);
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<StudentDTO>>> SearchStudents([FromQuery] string username)
    {
        List<StudentDTO> students = await _context.Students
            .Where(s => s.Username.Contains(username))
            .Select(student => MappingService.MapToStudentDTO(student))
            .ToListAsync();

        return Ok(students);
    }


    /// <summary>
    /// Gets a specific student by ID.
    /// </summary>
    /// <param name="id">The ID of the student to retrieve.</param>
    /// <returns>The detailed StudentDTO (including a list of bookings) if found, or a NotFound response.</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<StudentDTO>> GetStudent(int id)
    {
        Student? student = await _context.Students
            .Include(s => s.Bookings)
                .ThenInclude(b => b.Laptop)
            .Include(s => s.Bookings)
                .ThenInclude(b => b.Teacher)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (student == null)
        {
            return NotFound();
        }

        return Ok(MappingService.MapToStudentDetailDTO(student));
    }

    /// <summary>
    /// Creates a new student.
    /// </summary>
    /// <param name="studentDTO">The student DTO to create.</param>
    /// <returns>The created student DTO.</returns>
    [HttpPost]
    public async Task<ActionResult<StudentDTO>> PostStudent(StudentDTO studentDTO)
    {

        Student student = MappingService.MapToStudentModel(studentDTO);
        _context.Students.Add(student);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetStudent), new { id = student.Id }, MappingService.MapToStudentDTO(student));
    }

    /// <summary>
    /// Updates an existing student.
    /// </summary>
    /// <param name="id">The ID of the student to update.</param>
    /// <param name="studentDTO">The updated student DTO.</param>
    /// <returns>No content if successful, or an appropriate error response.</returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> PutStudent(int id, StudentDTO studentDTO)
    {
        if (id != studentDTO.Id)
        {
            return BadRequest("ID mismatch.");
        }

        Student? student = await _context.Students.FindAsync(id);
        if (student == null)
        {
            return NotFound();
        }

        student.Username = studentDTO.Username;

        _context.Entry(student).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!StudentExists(id))
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
    /// Deletes a student by ID.
    /// </summary>
    /// <param name="id">The ID of the student to delete.</param>
    /// <returns>No content if successful, or a NotFound response.</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteStudent(int id)
    {
        Student? student = await _context.Students.FindAsync(id);
        if (student == null)
        {
            return NotFound();
        }

        _context.Students.Remove(student);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool StudentExists(int id)
    {
        return _context.Students.Any(e => e.Id == id);
    }

}
