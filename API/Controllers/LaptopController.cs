using API.Models;
using API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.DTOs;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LaptopController : ControllerBase
{
    private readonly ITLendDBContext _context;

    public LaptopController(ITLendDBContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Gets the list of all laptops.
    /// </summary>
    /// <returns>A list of laptop DTOs.</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<LaptopDTO>>> GetLaptops()
    {
        List<LaptopDTO> laptops = await _context.Laptops
            .Select(laptop => MappingService.MapToLaptopDTO(laptop))
            .ToListAsync();

        return Ok(laptops);
    }

    /// <summary>
    /// Gets a specific laptop by ID.
    /// </summary>
    /// <param name="id">The ID of the laptop to retrieve.</param>
    /// <returns>The laptop DTO if found, or a NotFound response.</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<LaptopDTO>> GetLaptop(int id)
    {
        Laptop? laptop = await _context.Laptops.FindAsync(id);

        if (laptop == null)
        {
            return NotFound();
        }

        return Ok(MappingService.MapToLaptopDTO(laptop));
    }

    /// <summary>
    /// Creates a new laptop.
    /// </summary>
    /// <param name="laptopDTO">The laptop DTO to create.</param>
    /// <returns>The created laptop DTO.</returns>
    [HttpPost]
    public async Task<ActionResult<LaptopDTO>> PostLaptop(LaptopDTO laptopDTO)
    {

        Laptop laptop = MappingService.MapToLaptopModel(laptopDTO);
        _context.Laptops.Add(laptop);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetLaptop), new { id = laptop.Id }, MappingService.MapToLaptopDTO(laptop));
    }

    /// <summary>
    /// Updates an existing laptop.
    /// </summary>
    /// <param name="id">The ID of the laptop to update.</param>
    /// <param name="laptopDTO">The updated laptop DTO.</param>
    /// <returns>No content if successful, or an appropriate error response.</returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> PutLaptop(int id, LaptopDTO laptopDTO)
    {
        if (id != laptopDTO.Id)
        {
            return BadRequest("ID mismatch.");
        }

        Laptop? laptop = await _context.Laptops.FindAsync(id);
        if (laptop == null)
        {
            return NotFound();
        }

        laptop.IsAvailable = laptopDTO.IsAvailable;
        laptop.Model = laptopDTO.Model;
        laptop.IdentificationNumber = laptopDTO.IdentificationNumber;
        laptop.DamageDescription = laptopDTO.DamageDescription;

        _context.Entry(laptop).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!LaptopExists(id))
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
    /// Deletes a laptop by ID.
    /// </summary>
    /// <param name="id">The ID of the laptop to delete.</param>
    /// <returns>No content if successful, or a NotFound response.</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteLaptop(int id)
    {
        Laptop? laptop = await _context.Laptops.FindAsync(id);
        if (laptop == null)
        {
            return NotFound();
        }

        _context.Laptops.Remove(laptop);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool LaptopExists(int id)
    {
        return _context.Laptops.Any(e => e.Id == id);
    }
}
