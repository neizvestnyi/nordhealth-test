using Api.Data;
using Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Repositories;

public class AnimalRepository : Repository<Animal>, IAnimalRepository
{
    public AnimalRepository(VeterinaryDbContext context) : base(context)
    {
    }

    public override async Task<Animal?> GetByIdAsync(Guid id)
    {
        return await _dbSet
            .Include(a => a.Owner)
            .FirstOrDefaultAsync(a => a.Id == id);
    }
}