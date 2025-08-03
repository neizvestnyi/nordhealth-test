using Api.Data;
using Api.DTOs.Requests;
using Api.DTOs.Responses;
using Api.Models;
using Api.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AppointmentController : ControllerBase
{
    private readonly IAppointmentService _appointmentService;

    public AppointmentController(IAppointmentService appointmentService)
    {
        _appointmentService = appointmentService;
    }
    [HttpPost]
    public ActionResult<Animal> CreateAppointment([FromBody] Appointment appointment)
    {
        if (appointment == null)
        {
            return BadRequest("Appointment cannot be null.");
        }

        if (appointment.AnimalId == Guid.Empty || appointment.CustomerId == Guid.Empty)
        {
            return BadRequest("AnimalId and CustomerId are required.");
        }

        appointment.Id = Guid.NewGuid();

        AppointmentData.Appointments.Add(appointment);

        return CreatedAtAction(nameof(GetAppointment), new { id = appointment.Id }, appointment);
    }

    [HttpGet("{id}")]
    public ActionResult<Appointment> GetAppointment(Guid id)
    {
        var appointment = AppointmentData.Appointments.FirstOrDefault(a => a.Id == id);
        if (appointment == null)
        {
            return NotFound();
        }
        return Ok(appointment);
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
    public ActionResult<IEnumerable<AppointmentSummaryResponse>> GetVeterinarianAppointments(
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

        var result = _appointmentService.GetAppointmentsByVeterinarianAndDateRange(
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
    public ActionResult UpdateAppointmentStatus(
        [FromRoute] Guid id,
        [FromBody] UpdateAppointmentStatusRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = _appointmentService.UpdateAppointmentStatus(
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