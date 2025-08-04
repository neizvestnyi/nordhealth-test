using Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Data.Configurations;

public class VeterinarianConfiguration : IEntityTypeConfiguration<Veterinarian>
{
    public void Configure(EntityTypeBuilder<Veterinarian> builder)
    {
        builder.ToTable("Veterinarians");
        
        builder.HasKey(v => v.Id);
        
        builder.Property(v => v.FirstName)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(v => v.LastName)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(v => v.Email)
            .IsRequired()
            .HasMaxLength(256);
            
        builder.Property(v => v.PhoneNumber)
            .HasMaxLength(20);
            
        builder.Property(v => v.Specialization)
            .HasMaxLength(200);
            
        builder.Property(v => v.IsActive)
            .IsRequired()
            .HasDefaultValue(true);
            
        builder.HasMany(v => v.Appointments)
            .WithOne(a => a.Veterinarian)
            .HasForeignKey(a => a.VeterinarianId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasIndex(v => v.Email)
            .HasDatabaseName("IX_Veterinarians_Email")
            .IsUnique();
            
        builder.HasIndex(v => v.IsActive)
            .HasDatabaseName("IX_Veterinarians_IsActive");
            
        builder.Ignore(v => v.FullName);
    }
}