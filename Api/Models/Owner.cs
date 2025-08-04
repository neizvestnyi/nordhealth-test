namespace Api.Models;

public class Owner
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public required string Name { get; set; }
    
    public required string Email { get; set; }
    
    public string? PhoneNumber { get; set; }
    
    public string? Address { get; set; }
    
    public ICollection<Animal> Animals { get; set; } = new List<Animal>();
}