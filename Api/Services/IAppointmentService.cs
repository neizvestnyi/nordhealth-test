using Api.DTOs.Responses;

namespace Api.Services;

public interface IAppointmentService
{
    IEnumerable<AppointmentSummaryResponse> GetAppointmentsByVeterinarianAndDateRange(Guid veterinarianId, DateTime startDate, DateTime endDate);
}