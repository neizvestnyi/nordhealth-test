using Api.Models;

namespace Api.Data;

public static class DbInitializer
{
    public static void Initialize(VeterinaryDbContext context)
    {
        if (context.Veterinarians.Any())
        {
            return;
        }
        
        var veterinarians = new[]
        {
            new Veterinarian
            {
                Id = new Guid("e47ac10b-58cc-4372-a567-0e02b2c3d479"),
                FirstName = "John",
                LastName = "Smith",
                Email = "john.smith@vetclinic.com",
                PhoneNumber = "+1234567890",
                Specialization = "Small Animals",
                IsActive = true
            },
            new Veterinarian
            {
                Id = new Guid("e47ac10b-58cc-4372-a567-0e02b2c3d478"),
                FirstName = "Emily",
                LastName = "Johnson",
                Email = "emily.johnson@vetclinic.com",
                PhoneNumber = "+1234567891",
                Specialization = "Surgery",
                IsActive = true
            },
            new Veterinarian
            {
                Id = new Guid("e47ac10b-58cc-4372-a567-0e02b2c3d477"),
                FirstName = "Michael",
                LastName = "Brown",
                Email = "michael.brown@vetclinic.com",
                PhoneNumber = "+1234567892",
                Specialization = "Exotic Animals",
                IsActive = true
            }
        };
        
        context.Veterinarians.AddRange(veterinarians);
        
        var owners = new[]
        {
            new Owner
            {
                Id = new Guid("d47ac10b-58cc-4372-a567-0e02b2c3d479"),
                Name = "Dog Owner",
                Email = "dogowner@example.com",
                PhoneNumber = "+1234567893",
                Address = "123 Main St, City, State 12345"
            },
            new Owner
            {
                Id = new Guid("d47ac10b-58cc-4372-a567-0e02b2c3d478"),
                Name = "Cat Owner",
                Email = "catowner@example.com",
                PhoneNumber = "+1234567894",
                Address = "456 Oak Ave, City, State 12345"
            },
            new Owner
            {
                Id = new Guid("d47ac10b-58cc-4372-a567-0e02b2c3d477"),
                Name = "Rabbit Owner",
                Email = "rabbitsowner@example.com",
                PhoneNumber = "+1234567895",
                Address = "789 Pine Rd, City, State 12345"
            }
        };
        
        context.Owners.AddRange(owners);
        
        var animals = new[]
        {
            new Animal
            {
                Id = new Guid("f47ac10b-58cc-4372-a567-0e02b2c3d479"),
                Name = "Dog",
                BirthDate = DateTime.Now.AddYears(-3),
                OwnerId = owners[0].Id
            },
            new Animal
            {
                Id = new Guid("f47ac10b-58cc-4372-a567-0e02b2c3d477"),
                Name = "Cat",
                BirthDate = DateTime.Now.AddYears(-2),
                OwnerId = owners[1].Id
            },
            new Animal
            {
                Id = new Guid("f47ac10b-58cc-4372-a567-0e02b2c3d476"),
                Name = "Rabbit",
                BirthDate = DateTime.Now.AddYears(-1),
                OwnerId = owners[2].Id
            }
        };
        
        context.Animals.AddRange(animals);
        
        var appointments = new[]
        {
            new Appointment
            {
                Id = new Guid("a47ac10b-58cc-4372-a567-0e02b2c3d471"),
                StartTime = DateTime.Today.AddHours(9),
                EndTime = DateTime.Today.AddHours(10),
                AnimalId = animals[0].Id,
                VeterinarianId = veterinarians[0].Id,
                Status = AppointmentStatusEnum.Scheduled,
                Notes = "Annual checkup"
            },
            new Appointment
            {
                Id = new Guid("a47ac10b-58cc-4372-a567-0e02b2c3d472"),
                StartTime = DateTime.Today.AddHours(10),
                EndTime = DateTime.Today.AddHours(10).AddMinutes(30),
                AnimalId = animals[1].Id,
                VeterinarianId = veterinarians[0].Id,
                Status = AppointmentStatusEnum.Scheduled,
                Notes = "Follow-up visit"
            },
            new Appointment
            {
                Id = new Guid("a47ac10b-58cc-4372-a567-0e02b2c3d473"),
                StartTime = DateTime.Today.AddHours(11),
                EndTime = DateTime.Today.AddHours(12),
                AnimalId = animals[2].Id,
                VeterinarianId = veterinarians[1].Id,
                Status = AppointmentStatusEnum.InProgress,
                Notes = "Dental examination"
            },
            new Appointment
            {
                Id = new Guid("a47ac10b-58cc-4372-a567-0e02b2c3d474"),
                StartTime = DateTime.Today.AddDays(-1).AddHours(14),
                EndTime = DateTime.Today.AddDays(-1).AddHours(15),
                AnimalId = animals[0].Id,
                VeterinarianId = veterinarians[2].Id,
                Status = AppointmentStatusEnum.Completed,
                Notes = "Completed successfully"
            },
            new Appointment
            {
                Id = new Guid("a47ac10b-58cc-4372-a567-0e02b2c3d475"),
                StartTime = DateTime.Today.AddDays(1).AddHours(9),
                EndTime = DateTime.Today.AddDays(1).AddHours(9).AddMinutes(45),
                AnimalId = animals[1].Id,
                VeterinarianId = veterinarians[0].Id,
                Status = AppointmentStatusEnum.Scheduled,
                Notes = "Vaccination due"
            }
        };
        
        context.Appointments.AddRange(appointments);
        
        context.SaveChanges();
    }
}