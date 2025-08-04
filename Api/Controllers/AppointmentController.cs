using Api.DTOs.Requests;
using Api.DTOs.Responses;
using Api.Models;
using Api.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Api.Controllers;

/// <summary>
/// Manages veterinary appointments and scheduling
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AppointmentController : ControllerBase
{
    private readonly IAppointmentService _appointmentService;

    public AppointmentController(IAppointmentService appointmentService)
    {
        _appointmentService = appointmentService;
    }
    /// <summary>
    /// Creates a new appointment
    /// </summary>
    /// <param name="request">Appointment details including animal, veterinarian, and time</param>
    /// <returns>The created appointment with full details</returns>
    /// <response code="201">Appointment created successfully</response>
    /// <response code="400">Invalid request data or business rule violation</response>
    [HttpPost]
    [ProducesResponseType(typeof(Appointment), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Appointment>> CreateAppointment([FromBody] CreateAppointmentRequest request)
    {
        var result = await _appointmentService.CreateAppointmentAsync(request);
        
        if (!result.IsSuccess)
        {
            return Helpers.ResultHelper.ToActionResult(result);
        }

        return CreatedAtAction(nameof(GetAppointment), new { id = result.Data!.Id }, result.Data);
    }

    /// <summary>
    /// Retrieves an appointment by ID
    /// </summary>
    /// <param name="id">The unique identifier of the appointment</param>
    /// <returns>Appointment details with animal and veterinarian information</returns>
    /// <response code="200">Returns the requested appointment</response>
    /// <response code="404">Appointment not found</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Appointment), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Appointment>> GetAppointment(Guid id)
    {
        var result = await _appointmentService.GetAppointmentByIdAsync(id);
        
        if (!result.IsSuccess)
        {
            return Helpers.ResultHelper.ToActionResult(result);
        }

        return Ok(result.Data);
    }

    /// <summary>
    /// Retrieves appointments for a specific veterinarian within a date range
    /// </summary>
    /// <param name="veterinarianId">The unique identifier of the veterinarian</param>
    /// <param name="startDate">Start date of the search range</param>
    /// <param name="endDate">End date of the search range</param>
    /// <returns>List of appointments with animal and owner information</returns>
    /// <response code="200">Returns the list of appointments</response>
    /// <response code="400">Invalid request parameters or date range</response>
    [HttpGet("veterinarian/{veterinarianId}/appointments")]
    [ProducesResponseType(typeof(IEnumerable<AppointmentSummaryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<AppointmentSummaryResponse>>> GetVeterinarianAppointments(
        [FromRoute] Guid veterinarianId,
        [FromQuery, Required] DateTime startDate,
        [FromQuery, Required] DateTime endDate)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Validate date range
        if (endDate < startDate)
        {
            ModelState.AddModelError("endDate", "End date must be greater than or equal to start date");
            return BadRequest(ModelState);
        }

        var result = await _appointmentService.GetAppointmentsByVeterinarianAndDateRangeAsync(
            veterinarianId, 
            startDate, 
            endDate);

        if (!result.IsSuccess)
        {
            return Helpers.ResultHelper.ToActionResult(result);
        }

        return Ok(result.Data);
    }

    /// <summary>
    /// Updates the status of an appointment
    /// </summary>
    /// <param name="id">The appointment ID</param>
    /// <param name="request">The status update request</param>
    /// <returns>Success or error message</returns>
    /// <response code="200">Status updated successfully</response>
    /// <response code="400">Invalid request or business rule violation</response>
    /// <response code="404">Appointment not found</response>
    [HttpPatch("{id}/status")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UpdateAppointmentStatus(
        [FromRoute] Guid id,
        [FromBody] UpdateAppointmentStatusRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _appointmentService.UpdateAppointmentStatusAsync(
            id, 
            request.Status, 
            DateTime.Now);

        if (!result.IsSuccess)
        {
            return Helpers.ResultHelper.ToActionResult(result);
        }

        return Ok(new { message = "Appointment status updated successfully" });
    }
}