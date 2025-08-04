using Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Data.Configurations;

public class AnimalConfiguration : IEntityTypeConfiguration<Animal>
{
    public void Configure(EntityTypeBuilder<Animal> builder)
    {
        builder.ToTable("Animals");
        
        builder.HasKey(a => a.Id);
        
        builder.Property(a => a.Name)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(a => a.BirthDate)
            .IsRequired();
            
        builder.HasOne(a => a.Owner)
            .WithMany(o => o.Animals)
            .HasForeignKey(a => a.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasMany(a => a.Appointments)
            .WithOne(ap => ap.Animal)
            .HasForeignKey(ap => ap.AnimalId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasIndex(a => a.OwnerId)
            .HasDatabaseName("IX_Animals_OwnerId");
    }
}