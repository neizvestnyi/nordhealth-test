using Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Data.Configurations;

public class OwnerConfiguration : IEntityTypeConfiguration<Owner>
{
    public void Configure(EntityTypeBuilder<Owner> builder)
    {
        builder.ToTable("Owners");
        
        builder.HasKey(o => o.Id);
        
        builder.Property(o => o.Name)
            .IsRequired()
            .HasMaxLength(200);
            
        builder.Property(o => o.Email)
            .IsRequired()
            .HasMaxLength(256);
            
        builder.Property(o => o.PhoneNumber)
            .HasMaxLength(20);
            
        builder.Property(o => o.Address)
            .HasMaxLength(500);
            
        builder.HasMany(o => o.Animals)
            .WithOne(a => a.Owner)
            .HasForeignKey(a => a.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasIndex(o => o.Email)
            .HasDatabaseName("IX_Owners_Email")
            .IsUnique();
    }
}