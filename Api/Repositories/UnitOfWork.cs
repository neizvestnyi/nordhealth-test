using Api.Data;
using Api.Models;
using Microsoft.EntityFrameworkCore.Storage;

namespace Api.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly VeterinaryDbContext _context;
    private IDbContextTransaction? _transaction;
    
    private IAnimalRepository? _animalRepository;
    private IAppointmentRepository? _appointmentRepository;
    private IRepository<Owner>? _ownerRepository;
    private IRepository<Veterinarian>? _veterinarianRepository;

    public UnitOfWork(VeterinaryDbContext context)
    {
        _context = context;
    }

    public IAnimalRepository Animals => 
        _animalRepository ??= new AnimalRepository(_context);
        
    public IAppointmentRepository Appointments => 
        _appointmentRepository ??= new AppointmentRepository(_context);
        
    public IRepository<Owner> Owners => 
        _ownerRepository ??= new Repository<Owner>(_context);
        
    public IRepository<Veterinarian> Veterinarians => 
        _veterinarianRepository ??= new Repository<Veterinarian>(_context);

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}