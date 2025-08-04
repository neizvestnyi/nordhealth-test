using Api.Data;
using Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Repositories;

public class AppointmentRepository : Repository<Appointment>, IAppointmentRepository
{
    public AppointmentRepository(VeterinaryDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Appointment>> GetByVeterinarianAndDateRangeAsync(
        Guid veterinarianId, 
        DateTime startDate, 
        DateTime endDate)
    {
        return await _dbSet
            .Include(a => a.Animal)
                .ThenInclude(an => an.Owner)
            .Include(a => a.Veterinarian)
            .Where(a => a.VeterinarianId == veterinarianId)
            .Where(a => a.StartTime < endDate && a.EndTime > startDate)
            .OrderBy(a => a.StartTime)
            .ToListAsync();
    }


    public override async Task<Appointment?> GetByIdAsync(Guid id)
    {
        return await _dbSet
            .Include(a => a.Animal)
                .ThenInclude(an => an.Owner)
            .Include(a => a.Veterinarian)
            .FirstOrDefaultAsync(a => a.Id == id);
    }
}