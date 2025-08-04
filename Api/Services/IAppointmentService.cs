using Api.Common;
using Api.DTOs.Requests;
using Api.DTOs.Responses;
using Api.Models;

namespace Api.Services;

public interface IAppointmentService
{
    Task<Result<Appointment>> CreateAppointmentAsync(CreateAppointmentRequest request);
    Task<Result<Appointment>> GetAppointmentByIdAsync(Guid id);
    Task<Result<IEnumerable<AppointmentSummaryResponse>>> GetAppointmentsByVeterinarianAndDateRangeAsync(Guid veterinarianId, DateTime startDate, DateTime endDate);
    Task<Result> UpdateAppointmentStatusAsync(Guid appointmentId, AppointmentStatusEnum newStatus, DateTime currentTime);
}