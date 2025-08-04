using Api.Models;

namespace Api.Repositories;

/// <summary>
/// Repository interface for appointment-specific operations
/// </summary>
public interface IAppointmentRepository : IRepository<Appointment>
{
    Task<IEnumerable<Appointment>> GetByVeterinarianAndDateRangeAsync(
        Guid veterinarianId, 
        DateTime startDate, 
        DateTime endDate);
}