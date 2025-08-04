using Api.Models;

namespace Api.DTOs.Requests;

public class CreateAppointmentRequest
{
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public Guid AnimalId { get; set; }
    public Guid VeterinarianId { get; set; }
    public AppointmentStatusEnum Status { get; set; } = AppointmentStatusEnum.Scheduled;
    public string? Notes { get; set; }
}