using Api.Common;
using Api.Data;
using Api.DTOs.Responses;
using Api.Helpers;
using Api.Models;

namespace Api.Services;

public class AppointmentService : IAppointmentService
{
    private readonly INotificationService _notificationService;

    public AppointmentService(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }
    public Result<IEnumerable<AppointmentSummaryResponse>> GetAppointmentsByVeterinarianAndDateRange(Guid veterinarianId, DateTime startDate, DateTime endDate)
    {
        try
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

            return Result<IEnumerable<AppointmentSummaryResponse>>.Success(appointmentResponses);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<AppointmentSummaryResponse>>.Failure($"An error occurred while retrieving appointments: {ex.Message}", ErrorTypeEnum.InternalError);
        }
    }

    public Result UpdateAppointmentStatus(Guid appointmentId, AppointmentStatusEnum newStatus, DateTime currentTime)
    {
        try
        {
            var appointment = AppointmentData.Appointments.FirstOrDefault(a => a.Id == appointmentId);
            if (appointment == null)
            {
                return Result.Failure("Appointment not found", ErrorTypeEnum.NotFound);
            }

            // Migrate requested status if needed
            var migratedStatus = AppointmentStatusHelper.MigrateStatus(newStatus);

            if (!AppointmentStatusHelper.IsValidStatus(migratedStatus))
            {
                return Result.Failure(AppointmentStatusHelper.GetValidStatusesMessage(), ErrorTypeEnum.ValidationError);
            }

            if (migratedStatus == AppointmentStatusEnum.Cancelled)
            {
                if (!AppointmentStatusHelper.CanBeCancelled(appointment.StartTime, currentTime))
                {
                    return Result.Failure("Cannot cancel appointment within 1 hour of its start time", ErrorTypeEnum.BusinessRuleViolation);
                }
            }

            var previousStatus = appointment.Status;
            appointment.Status = migratedStatus;

            // Send notification if appointment was cancelled
            if (migratedStatus == AppointmentStatusEnum.Cancelled && previousStatus != AppointmentStatusEnum.Cancelled)
            {
                var animal = AnimalData.Animals.FirstOrDefault(a => a.Id == appointment.AnimalId);
                if (animal != null)
                {
                    _notificationService.SendAppointmentCancellationEmail(
                        animal.OwnerEmail,
                        animal.Name,
                        appointment.StartTime);
                }
            }

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"An error occurred while updating appointment status: {ex.Message}", ErrorTypeEnum.InternalError);
        }
    }
}