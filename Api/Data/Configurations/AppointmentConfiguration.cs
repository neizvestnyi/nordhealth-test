using Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Data.Configurations;

public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.ToTable("Appointments");
        
        builder.HasKey(a => a.Id);
        
        builder.Property(a => a.StartTime)
            .IsRequired();
            
        builder.Property(a => a.EndTime)
            .IsRequired();
            
        builder.Property(a => a.Status)
            .IsRequired()
            .HasConversion<string>() // Store enum as string for readability
            .HasMaxLength(20);
            
        builder.Property(a => a.Notes)
            .HasMaxLength(1000);
            
        builder.HasOne(a => a.Animal)
            .WithMany(an => an.Appointments)
            .HasForeignKey(a => a.AnimalId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasOne(a => a.Veterinarian)
            .WithMany(v => v.Appointments)
            .HasForeignKey(a => a.VeterinarianId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasIndex(a => a.VeterinarianId)
            .HasDatabaseName("IX_Appointments_VeterinarianId");
            
        builder.HasIndex(a => a.AnimalId)
            .HasDatabaseName("IX_Appointments_AnimalId");
            
        builder.HasIndex(a => new { a.StartTime, a.EndTime })
            .HasDatabaseName("IX_Appointments_TimeRange");
            
        builder.HasIndex(a => a.Status)
            .HasDatabaseName("IX_Appointments_Status");
            
        builder.HasCheckConstraint("CK_Appointments_EndTimeAfterStartTime", 
            "[EndTime] > [StartTime]");
    }
}