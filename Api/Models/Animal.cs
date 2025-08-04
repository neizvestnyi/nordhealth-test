namespace Api.Models;

public class Animal
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public required string Name { get; set; }

    public DateTime BirthDate { get; set; }
    
    public Guid OwnerId { get; set; }
    
    public Owner Owner { get; set; } = null!;
    
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}