using Api.Common;
using Api.DTOs.Responses;
using Api.Models;

namespace Api.Services;

public interface IAppointmentService
{
    IEnumerable<AppointmentSummaryResponse> GetAppointmentsByVeterinarianAndDateRange(Guid veterinarianId, DateTime startDate, DateTime endDate);
    Result UpdateAppointmentStatus(Guid appointmentId, AppointmentStatus newStatus, DateTime currentTime);
}