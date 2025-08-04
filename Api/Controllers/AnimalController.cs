using Api.DTOs.Requests;
using Api.Models;
using Api.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AnimalController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public AnimalController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpPost]
    public async Task<ActionResult<Animal>> CreateAnimal([FromBody] CreateAnimalRequest request)
    {
        if (request == null)
        {
            return BadRequest("Animal request cannot be null.");
        }

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return BadRequest("Animal name is required.");
        }

        var animal = new Animal
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            BirthDate = request.BirthDate,
            OwnerId = request.OwnerId
        };

        await _unitOfWork.Animals.AddAsync(animal);
        await _unitOfWork.SaveChangesAsync();
        
        // Reload with navigation properties
        var createdAnimal = await _unitOfWork.Animals.GetByIdAsync(animal.Id);

        return CreatedAtAction(nameof(GetAnimal), new { id = animal.Id }, createdAnimal);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Animal>> GetAnimal(Guid id)
    {
        var animal = await _unitOfWork.Animals.GetByIdAsync(id);
        return Ok(animal);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAnimal(Guid id)
    {
        var animal = await _unitOfWork.Animals.GetByIdAsync(id);
        if (animal == null)
        {
            return NotFound("Animal not found.");
        }

        await _unitOfWork.Animals.DeleteAsync(animal);
        await _unitOfWork.SaveChangesAsync();
        return NoContent();
    }
}