using Api.Models;

namespace Api.DTOs.Responses;

/// <summary>
/// Response model for appointment summary with animal and owner information
/// </summary>
public class AppointmentSummaryResponse
{
    /// <summary>
    /// Unique identifier of the appointment
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Appointment start time
    /// </summary>
    public DateTime StartTime { get; set; }
    
    /// <summary>
    /// Appointment end time
    /// </summary>
    public DateTime EndTime { get; set; }
    
    /// <summary>
    /// Name of the animal
    /// </summary>
    public string AnimalName { get; set; } = string.Empty;
    
    /// <summary>
    /// Name of the animal's owner
    /// </summary>
    public string OwnerName { get; set; } = string.Empty;
    
    /// <summary>
    /// Current status of the appointment
    /// </summary>
    public AppointmentStatus Status { get; set; }
}