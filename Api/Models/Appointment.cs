namespace Api.Models;

public class Appointment
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public required DateTime StartTime { get; set; }

    public required DateTime EndTime { get; set; }

    public AppointmentStatusEnum Status { get; set; } = AppointmentStatusEnum.Scheduled;

    public string? Notes { get; set; }
    
    public required Guid AnimalId { get; set; }
    
    public required Guid VeterinarianId { get; set; }
    
    public Animal Animal { get; set; } = null!;
    
    public Veterinarian Veterinarian { get; set; } = null!;

    // Constructor that satisfies all required properties
    public Appointment(DateTime startTime, DateTime endTime, Guid animalId, Guid veterinarianId)
    {
        if (endTime <= startTime)
        {
            throw new ArgumentException("End time must be after start time");
        }

        StartTime = startTime;
        EndTime = endTime;
        AnimalId = animalId;
        VeterinarianId = veterinarianId;
    }

    // Required for EF Core and object initializer syntax
    [Obsolete("Use constructor or object initializer with all required properties")]
    public Appointment() { }
}