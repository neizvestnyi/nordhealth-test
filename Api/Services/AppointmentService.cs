using Api.Common;
using Api.DTOs.Requests;
using Api.DTOs.Responses;
using Api.Helpers;
using Api.Models;
using Api.Repositories;
using Microsoft.Extensions.Logging;

namespace Api.Services;

public class AppointmentService : IAppointmentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationService _notificationService;
    private readonly ILogger<AppointmentService> _logger;

    public AppointmentService(
        IUnitOfWork unitOfWork, 
        INotificationService notificationService,
        ILogger<AppointmentService> logger)
    {
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task<Result<Appointment>> CreateAppointmentAsync(CreateAppointmentRequest request)
    {
        try
        {
            if (request == null)
            {
                return Result<Appointment>.Failure("Appointment request cannot be null.", ErrorTypeEnum.ValidationError);
            }

            if (request.AnimalId == Guid.Empty || request.VeterinarianId == Guid.Empty)
            {
                return Result<Appointment>.Failure("AnimalId and VeterinarianId are required.", ErrorTypeEnum.ValidationError);
            }

            var appointment = new Appointment
            {
                Id = Guid.NewGuid(),
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                AnimalId = request.AnimalId,
                VeterinarianId = request.VeterinarianId,
                Status = request.Status,
                Notes = request.Notes
            };

            await _unitOfWork.Appointments.AddAsync(appointment);
            await _unitOfWork.SaveChangesAsync();

            // Reload with navigation properties
            var createdAppointment = await _unitOfWork.Appointments.GetByIdAsync(appointment.Id);

            return Result<Appointment>.Success(createdAppointment!);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating appointment for Animal {AnimalId}", request?.AnimalId);
            return Result<Appointment>.Failure($"An error occurred while creating appointment: {ex.Message}", ErrorTypeEnum.InternalError);
        }
    }

    public async Task<Result<Appointment>> GetAppointmentByIdAsync(Guid id)
    {
        try
        {
            var appointment = await _unitOfWork.Appointments.GetByIdAsync(id);
            if (appointment == null)
            {
                return Result<Appointment>.Failure("Appointment not found.", ErrorTypeEnum.NotFound);
            }

            return Result<Appointment>.Success(appointment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving appointment {AppointmentId}", id);
            return Result<Appointment>.Failure($"An error occurred while retrieving appointment: {ex.Message}", ErrorTypeEnum.InternalError);
        }
    }

    public async Task<Result<IEnumerable<AppointmentSummaryResponse>>> GetAppointmentsByVeterinarianAndDateRangeAsync(Guid veterinarianId, DateTime startDate, DateTime endDate)
    {
        try
        {
            var appointments = await _unitOfWork.Appointments
                .GetByVeterinarianAndDateRangeAsync(veterinarianId, startDate, endDate);

            var appointmentResponses = appointments.Select(appointment => new AppointmentSummaryResponse
            {
                Id = appointment.Id,
                StartTime = appointment.StartTime,
                EndTime = appointment.EndTime,
                AnimalName = appointment.Animal?.Name ?? "Unknown",
                OwnerName = appointment.Animal?.Owner?.Name ?? "Unknown",
                Status = appointment.Status
            })
            .ToList();

            return Result<IEnumerable<AppointmentSummaryResponse>>.Success(appointmentResponses);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving appointments for Veterinarian {VeterinarianId}", veterinarianId);
            return Result<IEnumerable<AppointmentSummaryResponse>>.Failure($"An error occurred while retrieving appointments: {ex.Message}", ErrorTypeEnum.InternalError);
        }
    }

    public async Task<Result> UpdateAppointmentStatusAsync(Guid appointmentId, AppointmentStatusEnum newStatus, DateTime currentTime)
    {
        try
        {
            var appointment = await _unitOfWork.Appointments.GetByIdAsync(appointmentId);
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

            await _unitOfWork.Appointments.UpdateAsync(appointment);
            await _unitOfWork.SaveChangesAsync();

            // Send notification if appointment was cancelled
            if (migratedStatus == AppointmentStatusEnum.Cancelled && previousStatus != AppointmentStatusEnum.Cancelled)
            {
                if (appointment.Animal?.Owner != null)
                {
                    _notificationService.SendAppointmentCancellationEmail(
                        appointment.Animal.Owner.Email,
                        appointment.Animal.Name,
                        appointment.StartTime);
                }
            }

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating appointment {AppointmentId} status", appointmentId);
            return Result.Failure($"An error occurred while updating appointment status: {ex.Message}", ErrorTypeEnum.InternalError);
        }
    }
}