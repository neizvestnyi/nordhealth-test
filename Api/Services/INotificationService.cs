namespace Api.Services;

public interface INotificationService
{
    void SendAppointmentCancellationEmail(string ownerEmail, string animalName, DateTime appointmentTime);
}