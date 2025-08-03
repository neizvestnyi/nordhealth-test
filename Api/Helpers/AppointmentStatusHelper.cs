using Api.Models;

namespace Api.Helpers;

public static class AppointmentStatusHelper
{
    /// <summary>
    /// Valid statuses according to business requirements
    /// </summary>
    private static readonly HashSet<AppointmentStatus> ValidStatuses = new()
    {
        AppointmentStatus.Scheduled,
        AppointmentStatus.Completed,
        AppointmentStatus.Cancelled
    };

    /// <summary>
    /// Migrates old status values to valid ones according to business rules
    /// </summary>
    public static AppointmentStatus MigrateStatus(AppointmentStatus currentStatus)
    {
        return currentStatus switch
        {
            AppointmentStatus.InProgress => AppointmentStatus.Scheduled,
            AppointmentStatus.NoShow => AppointmentStatus.Cancelled,
            _ => currentStatus
        };
    }

    /// <summary>
    /// Checks if a status is valid according to business requirements
    /// </summary>
    public static bool IsValidStatus(AppointmentStatus status)
    {
        return ValidStatuses.Contains(status);
    }

    /// <summary>
    /// Gets a formatted string of valid statuses for error messages
    /// </summary>
    public static string GetValidStatusesMessage()
    {
        return $"Invalid status. Valid statuses are: {string.Join(", ", ValidStatuses)}";
    }

    /// <summary>
    /// Validates if an appointment can be cancelled based on its start time
    /// </summary>
    public static bool CanBeCancelled(DateTime appointmentStartTime, DateTime currentTime)
    {
        var timeUntilAppointment = appointmentStartTime - currentTime;
        return timeUntilAppointment.TotalHours >= 1;
    }
}