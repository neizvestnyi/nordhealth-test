using Api.DTOs.Requests;
using Api.Helpers;
using Api.Models;
using Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>
/// Manages animal (pet) records in the veterinary system
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AnimalController : ControllerBase
{
    private readonly IAnimalService _animalService;

    public AnimalController(IAnimalService animalService)
    {
        _animalService = animalService;
    }

    /// <summary>
    /// Creates a new animal record
    /// </summary>
    /// <param name="request">Animal creation details</param>
    /// <returns>The created animal with owner information</returns>
    /// <response code="201">Animal created successfully</response>
    /// <response code="400">Invalid request data</response>
    [HttpPost]
    [ProducesResponseType(typeof(Animal), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Animal>> CreateAnimal([FromBody] CreateAnimalRequest request)
    {
        var result = await _animalService.CreateAnimalAsync(request);
        
        if (!result.IsSuccess)
        {
            return ResultHelper.ToActionResult(result);
        }

        return CreatedAtAction(nameof(GetAnimal), new { id = result.Data!.Id }, result.Data);
    }

    /// <summary>
    /// Retrieves an animal by ID
    /// </summary>
    /// <param name="id">The unique identifier of the animal</param>
    /// <returns>Animal details with owner information</returns>
    /// <response code="200">Returns the requested animal</response>
    /// <response code="404">Animal not found</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Animal), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Animal>> GetAnimal(Guid id)
    {
        var result = await _animalService.GetAnimalByIdAsync(id);
        
        if (!result.IsSuccess)
        {
            return ResultHelper.ToActionResult(result);
        }

        return Ok(result.Data);
    }

    /// <summary>
    /// Deletes an animal record
    /// </summary>
    /// <param name="id">The unique identifier of the animal to delete</param>
    /// <returns>No content on success</returns>
    /// <response code="204">Animal deleted successfully</response>
    /// <response code="404">Animal not found</response>
    /// <response code="500">Unable to delete due to existing appointments</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteAnimal(Guid id)
    {
        var result = await _animalService.DeleteAnimalAsync(id);
        
        if (!result.IsSuccess)
        {
            return ResultHelper.ToActionResult(result);
        }

        return NoContent();
    }
}