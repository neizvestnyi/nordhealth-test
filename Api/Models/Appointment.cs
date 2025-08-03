namespace Api.Models;

public class Appointment
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public required DateTime StartTime { get; set; }

    public required DateTime EndTime { get; set; }

    public required Guid AnimalId { get; set; }

    public required Guid CustomerId { get; set; }

    public required Guid VeterinarianId { get; set; }

    public AppointmentStatusEnum Status { get; set; } = AppointmentStatusEnum.Scheduled;

    public string? Notes { get; set; }

    // Constructor that satisfies all required properties
    public Appointment(DateTime startTime, DateTime endTime, Guid animalId, Guid customerId, Guid veterinarianId)
    {
        if (endTime <= startTime)
        {
            throw new ArgumentException("End time must be after start time");
        }

        StartTime = startTime;
        EndTime = endTime;
        AnimalId = animalId;
        CustomerId = customerId;
        VeterinarianId = veterinarianId;
    }

    // Required for object initializer syntax with required properties
    [Obsolete("Use constructor or object initializer with all required properties")]
    public Appointment() { }
}