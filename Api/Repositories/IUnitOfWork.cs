namespace Api.Repositories;

public interface IUnitOfWork : IDisposable
{
    IAnimalRepository Animals { get; }
    IAppointmentRepository Appointments { get; }
    IRepository<Models.Owner> Owners { get; }
    IRepository<Models.Veterinarian> Veterinarians { get; }
    
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}