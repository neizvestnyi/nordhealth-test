namespace Api.Models;

public enum AppointmentStatusEnum
{
    Scheduled,
    [Obsolete("Use Scheduled instead. This status is being migrated.")]
    InProgress,
    Completed,
    Cancelled,
    [Obsolete("Use Cancelled instead. This status is being migrated.")]
    NoShow
}