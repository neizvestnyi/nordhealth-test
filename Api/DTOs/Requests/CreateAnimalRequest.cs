namespace Api.DTOs.Requests;

public class CreateAnimalRequest
{
    public required string Name { get; set; }
    public DateTime BirthDate { get; set; }
    public Guid OwnerId { get; set; }
}