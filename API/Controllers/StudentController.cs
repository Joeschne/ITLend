using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
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
        /// Gets the list of all students.
        /// </summary>
        /// <returns>A list of student DTOs.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StudentDTO>>> GetStudents()
        {
            List<StudentDTO> students = await _context.Students
                .Select(student => MapToDTO(student))
                .ToListAsync();

            return Ok(students);
        }

        /// <summary>
        /// Gets a specific student by ID.
        /// </summary>
        /// <param name="id">The ID of the student to retrieve.</param>
        /// <returns>The student DTO if found, or a NotFound response.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<StudentDTO>> GetStudent(int id)
        {
            Student? student = await _context.Students.FindAsync(id);

            if (student == null)
            {
                return NotFound();
            }

            return Ok(MapToDTO(student));
        }

        /// <summary>
        /// Creates a new student.
        /// </summary>
        /// <param name="studentDTO">The student DTO to create.</param>
        /// <returns>The created student DTO.</returns>
        [HttpPost]
        public async Task<ActionResult<StudentDTO>> PostStudent(StudentDTO studentDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Student student = MapToModel(studentDTO);
            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetStudent), new { id = student.Id }, MapToDTO(student));
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

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Student? student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }

            student.Username = studentDTO.Username;
            student.FirstName = studentDTO.FirstName;
            student.LastName = studentDTO.LastName;
            student.Email = studentDTO.Email;
            student.MobilePhoneNumber = studentDTO.MobilePhoneNumber;
            student.Gender = studentDTO.Gender;

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

        /// <summary>
        /// Checks if a student exists by ID.
        /// </summary>
        /// <param name="id">The ID of the student to check.</param>
        /// <returns>True if the student exists, otherwise false.</returns>
        private bool StudentExists(int id)
        {
            return _context.Students.Any(e => e.Id == id);
        }

        /// <summary>
        /// Maps a Student entity to a StudentDTO.
        /// </summary>
        /// <param name="student">The Student entity to map.</param>
        /// <returns>The mapped StudentDTO.</returns>
        private static StudentDTO MapToDTO(Student student)
        {
            return new StudentDTO
            {
                Id = student.Id,
                Username = student.Username,
                FirstName = student.FirstName,
                LastName = student.LastName,
                Email = student.Email,
                MobilePhoneNumber = student.MobilePhoneNumber,
                Gender = student.Gender
            };
        }

        /// <summary>
        /// Maps a StudentDTO to a Student entity.
        /// </summary>
        /// <param name="studentDTO">The StudentDTO to map.</param>
        /// <returns>The mapped Student entity.</returns>
        private static Student MapToModel(StudentDTO studentDTO)
        {
            return new Student
            {
                Id = studentDTO.Id,
                Username = studentDTO.Username,
                FirstName = studentDTO.FirstName,
                LastName = studentDTO.LastName,
                Email = studentDTO.Email,
                MobilePhoneNumber = studentDTO.MobilePhoneNumber,
                Gender = studentDTO.Gender
            };
        }
    }
}
