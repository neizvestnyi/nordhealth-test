using Api.Data;
using Api.DTOs.Responses;
using Api.Helpers;

namespace Api.Services;

public class AppointmentService : IAppointmentService
{
    public IEnumerable<AppointmentSummaryResponse> GetAppointmentsByVeterinarianAndDateRange(Guid veterinarianId, DateTime startDate, DateTime endDate)
    {
        var filteredAppointments = AppointmentData.Appointments
            .Where(a => a.VeterinarianId == veterinarianId)
            .Where(a => DateRangeHelper.IsOverlapping(a.StartTime, a.EndTime, startDate, endDate))
            .ToList();

        var appointmentResponses = filteredAppointments.Select(appointment =>
        {
            var animal = AnimalData.Animals.FirstOrDefault(an => an.Id == appointment.AnimalId);
            
            return new AppointmentSummaryResponse
            {
                Id = appointment.Id,
                StartTime = appointment.StartTime,
                EndTime = appointment.EndTime,
                AnimalName = animal?.Name ?? "Unknown",
                OwnerName = animal?.OwnerName ?? "Unknown",
                Status = appointment.Status
            };
        })
        .OrderBy(a => a.StartTime)
        .ToList();

        return appointmentResponses;
    }
}