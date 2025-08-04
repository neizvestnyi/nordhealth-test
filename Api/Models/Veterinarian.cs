namespace Api.Models;

public class Veterinarian
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public required string FirstName { get; set; }
    
    public required string LastName { get; set; }
    
    public required string Email { get; set; }
    
    public string? PhoneNumber { get; set; }
    
    public string? Specialization { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    
    public string FullName => $"{FirstName} {LastName}";
}