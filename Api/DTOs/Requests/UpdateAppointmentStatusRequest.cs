using System.ComponentModel.DataAnnotations;
using Api.Models;

namespace Api.DTOs.Requests;

/// <summary>
/// Request model for updating appointment status
/// </summary>
public class UpdateAppointmentStatusRequest
{
    /// <summary>
    /// The new status for the appointment
    /// </summary>
    [Required(ErrorMessage = "Status is required")]
    [EnumDataType(typeof(AppointmentStatus), ErrorMessage = "Invalid appointment status")]
    public AppointmentStatus Status { get; set; }
}