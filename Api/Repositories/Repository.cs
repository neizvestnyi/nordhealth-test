using Api.Data;
using Microsoft.EntityFrameworkCore;

namespace Api.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly VeterinaryDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(VeterinaryDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public virtual async Task<T?> GetByIdAsync(Guid id)
    {
        return await _dbSet.FindAsync(id);
    }


    public virtual async Task<T> AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        return entity;
    }

    public virtual async Task UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        await Task.CompletedTask;
    }

    public virtual async Task DeleteAsync(T entity)
    {
        _dbSet.Remove(entity);
        await Task.CompletedTask;
    }

}