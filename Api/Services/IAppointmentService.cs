using Api.Common;
using Api.DTOs.Responses;
using Api.Models;

namespace Api.Services;

public interface IAppointmentService
{
    Result<IEnumerable<AppointmentSummaryResponse>> GetAppointmentsByVeterinarianAndDateRange(Guid veterinarianId, DateTime startDate, DateTime endDate);
    Result UpdateAppointmentStatus(Guid appointmentId, AppointmentStatusEnum newStatus, DateTime currentTime);
}