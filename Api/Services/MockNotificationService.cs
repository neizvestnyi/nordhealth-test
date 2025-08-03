namespace Api.Services;

public class MockNotificationService : INotificationService
{
    public void SendAppointmentCancellationEmail(string ownerEmail, string animalName, DateTime appointmentTime)
    {
        Console.WriteLine($"Email sent to {ownerEmail}");
        Console.WriteLine($"Subject: Appointment Cancellation Notice");
        Console.WriteLine($"Dear Pet Owner,");
        Console.WriteLine($"Your appointment for {animalName} scheduled on {appointmentTime:yyyy-MM-dd HH:mm} has been cancelled.");
        Console.WriteLine($"Please contact us to reschedule.");
    }
}