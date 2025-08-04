using Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Data;

public class VeterinaryDbContext : DbContext
{
    public VeterinaryDbContext(DbContextOptions<VeterinaryDbContext> options) 
        : base(options)
    {
    }
    
    public DbSet<Animal> Animals { get; set; }
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<Owner> Owners { get; set; }
    public DbSet<Veterinarian> Veterinarians { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(VeterinaryDbContext).Assembly);
    }
    
}