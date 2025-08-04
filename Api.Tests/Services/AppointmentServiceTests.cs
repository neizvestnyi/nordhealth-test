using Api.Common;
using Api.DTOs.Requests;
using Api.DTOs.Responses;
using Api.Helpers;
using Api.Models;
using Api.Repositories;
using Api.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace Api.Tests.Services;

public class AppointmentServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IAppointmentRepository> _mockAppointmentRepository;
    private readonly Mock<INotificationService> _mockNotificationService;
    private readonly Mock<ILogger<AppointmentService>> _mockLogger;
    private readonly AppointmentService _service;

    public AppointmentServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockAppointmentRepository = new Mock<IAppointmentRepository>();
        _mockNotificationService = new Mock<INotificationService>();
        _mockLogger = new Mock<ILogger<AppointmentService>>();
        
        _mockUnitOfWork.Setup(x => x.Appointments).Returns(_mockAppointmentRepository.Object);
        
        _service = new AppointmentService(
            _mockUnitOfWork.Object, 
            _mockNotificationService.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task CreateAppointmentAsync_WithValidRequest_ReturnsSuccess()
    {
        // Arrange
        var request = new CreateAppointmentRequest
        {
            AnimalId = Guid.NewGuid(),
            VeterinarianId = Guid.NewGuid(),
            StartTime = DateTime.Now.AddDays(1),
            EndTime = DateTime.Now.AddDays(1).AddHours(1),
            Status = AppointmentStatusEnum.Scheduled,
            Notes = "Regular checkup"
        };

        var createdAppointment = new Appointment
        {
            Id = Guid.NewGuid(),
            AnimalId = request.AnimalId,
            VeterinarianId = request.VeterinarianId,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            Status = request.Status,
            Notes = request.Notes,
            Animal = new Animal { Name = "Fluffy" },
            Veterinarian = new Veterinarian { FirstName = "John", LastName = "Smith", Email = "john.smith@vet.com", PhoneNumber = "+1234567890", Specialization = "General" }
        };

        _mockAppointmentRepository.Setup(x => x.AddAsync(It.IsAny<Appointment>()))
            .ReturnsAsync((Appointment a) => a);
        _mockUnitOfWork.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);
        _mockAppointmentRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(createdAppointment);

        // Act
        var result = await _service.CreateAppointmentAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.AnimalId.Should().Be(request.AnimalId);
        result.Data.VeterinarianId.Should().Be(request.VeterinarianId);
        result.Data.StartTime.Should().Be(request.StartTime);
        result.Data.EndTime.Should().Be(request.EndTime);
        
        _mockAppointmentRepository.Verify(x => x.AddAsync(It.IsAny<Appointment>()), Times.Once);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateAppointmentAsync_WithNullRequest_ReturnsValidationError()
    {
        // Act
        var result = await _service.CreateAppointmentAsync(null!);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorType.Should().Be(ErrorTypeEnum.ValidationError);
        result.ErrorMessage.Should().Contain("cannot be null");
    }

    [Fact]
    public async Task CreateAppointmentAsync_WithEmptyAnimalId_ReturnsValidationError()
    {
        // Arrange
        var request = new CreateAppointmentRequest
        {
            AnimalId = Guid.Empty,
            VeterinarianId = Guid.NewGuid(),
            StartTime = DateTime.Now.AddDays(1),
            EndTime = DateTime.Now.AddDays(1).AddHours(1)
        };

        // Act
        var result = await _service.CreateAppointmentAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorType.Should().Be(ErrorTypeEnum.ValidationError);
        result.ErrorMessage.Should().Contain("AnimalId and VeterinarianId are required");
    }

    [Fact]
    public async Task CreateAppointmentAsync_WhenExceptionOccurs_ReturnsInternalError()
    {
        // Arrange
        var request = new CreateAppointmentRequest
        {
            AnimalId = Guid.NewGuid(),
            VeterinarianId = Guid.NewGuid(),
            StartTime = DateTime.Now.AddDays(1),
            EndTime = DateTime.Now.AddDays(1).AddHours(1)
        };

        _mockAppointmentRepository.Setup(x => x.AddAsync(It.IsAny<Appointment>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _service.CreateAppointmentAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorType.Should().Be(ErrorTypeEnum.InternalError);
        result.ErrorMessage.Should().Contain("error occurred while creating appointment");
    }

    [Fact]
    public async Task GetAppointmentByIdAsync_WithExistingId_ReturnsSuccess()
    {
        // Arrange
        var appointmentId = Guid.NewGuid();
        var appointment = new Appointment
        {
            Id = appointmentId,
            AnimalId = Guid.NewGuid(),
            VeterinarianId = Guid.NewGuid(),
            StartTime = DateTime.Now.AddDays(1),
            EndTime = DateTime.Now.AddDays(1).AddHours(1),
            Status = AppointmentStatusEnum.Scheduled
        };

        _mockAppointmentRepository.Setup(x => x.GetByIdAsync(appointmentId))
            .ReturnsAsync(appointment);

        // Act
        var result = await _service.GetAppointmentByIdAsync(appointmentId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Id.Should().Be(appointmentId);
    }

    [Fact]
    public async Task GetAppointmentByIdAsync_WithNonExistingId_ReturnsNotFound()
    {
        // Arrange
        var appointmentId = Guid.NewGuid();
        _mockAppointmentRepository.Setup(x => x.GetByIdAsync(appointmentId))
            .ReturnsAsync((Appointment?)null);

        // Act
        var result = await _service.GetAppointmentByIdAsync(appointmentId);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorType.Should().Be(ErrorTypeEnum.NotFound);
        result.ErrorMessage.Should().Contain("Appointment not found");
    }

    [Fact]
    public async Task GetAppointmentsByVeterinarianAndDateRangeAsync_ReturnsAppointmentSummaries()
    {
        // Arrange
        var veterinarianId = Guid.NewGuid();
        var startDate = DateTime.Today;
        var endDate = DateTime.Today.AddDays(7);

        var appointments = new List<Appointment>
        {
            new Appointment
            {
                Id = Guid.NewGuid(),
                AnimalId = Guid.NewGuid(),
                VeterinarianId = veterinarianId,
                StartTime = startDate.AddHours(9),
                EndTime = startDate.AddHours(10),
                Status = AppointmentStatusEnum.Scheduled,
                Animal = new Animal 
                { 
                    Name = "Fluffy",
                    Owner = new Owner { Name = "John Doe", Email = "john@example.com", PhoneNumber = "+1234567890", Address = "123 Main St" }
                }
            },
            new Appointment
            {
                Id = Guid.NewGuid(),
                AnimalId = Guid.NewGuid(),
                VeterinarianId = veterinarianId,
                StartTime = startDate.AddDays(1).AddHours(14),
                EndTime = startDate.AddDays(1).AddHours(15),
                Status = AppointmentStatusEnum.Completed,
                Animal = new Animal 
                { 
                    Name = "Buddy",
                    Owner = new Owner { Name = "Jane Smith", Email = "jane@example.com", PhoneNumber = "+1234567891", Address = "456 Oak St" }
                }
            }
        };

        _mockAppointmentRepository.Setup(x => x.GetByVeterinarianAndDateRangeAsync(veterinarianId, startDate, endDate))
            .ReturnsAsync(appointments);

        // Act
        var result = await _service.GetAppointmentsByVeterinarianAndDateRangeAsync(veterinarianId, startDate, endDate);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        var summaries = result.Data!.ToList();
        summaries.Should().HaveCount(2);
        
        summaries[0].AnimalName.Should().Be("Fluffy");
        summaries[0].OwnerName.Should().Be("John Doe");
        summaries[0].Status.Should().Be(AppointmentStatusEnum.Scheduled);
        
        summaries[1].AnimalName.Should().Be("Buddy");
        summaries[1].OwnerName.Should().Be("Jane Smith");
        summaries[1].Status.Should().Be(AppointmentStatusEnum.Completed);
    }

    [Fact]
    public async Task GetAppointmentsByVeterinarianAndDateRangeAsync_WithMissingAnimalData_ReturnsUnknown()
    {
        // Arrange
        var veterinarianId = Guid.NewGuid();
        var appointments = new List<Appointment>
        {
            new Appointment
            {
                Id = Guid.NewGuid(),
                AnimalId = Guid.NewGuid(),
                VeterinarianId = veterinarianId,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(1),
                Status = AppointmentStatusEnum.Scheduled,
                Animal = null! // Missing animal
            }
        };

        _mockAppointmentRepository.Setup(x => x.GetByVeterinarianAndDateRangeAsync(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync(appointments);

        // Act
        var result = await _service.GetAppointmentsByVeterinarianAndDateRangeAsync(veterinarianId, DateTime.Today, DateTime.Today.AddDays(1));

        // Assert
        result.IsSuccess.Should().BeTrue();
        var summaries = result.Data!.ToList();
        summaries[0].AnimalName.Should().Be("Unknown");
        summaries[0].OwnerName.Should().Be("Unknown");
    }

    [Fact]
    public async Task UpdateAppointmentStatusAsync_ToCompleted_ReturnsSuccess()
    {
        // Arrange
        var appointmentId = Guid.NewGuid();
        var appointment = new Appointment
        {
            Id = appointmentId,
            AnimalId = Guid.NewGuid(),
            VeterinarianId = Guid.NewGuid(),
            StartTime = DateTime.Now.AddHours(-2),
            EndTime = DateTime.Now.AddHours(-1),
            Status = AppointmentStatusEnum.Scheduled
        };

        _mockAppointmentRepository.Setup(x => x.GetByIdAsync(appointmentId))
            .ReturnsAsync(appointment);
        _mockAppointmentRepository.Setup(x => x.UpdateAsync(It.IsAny<Appointment>()))
            .Returns(Task.CompletedTask);
        _mockUnitOfWork.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _service.UpdateAppointmentStatusAsync(appointmentId, AppointmentStatusEnum.Completed, DateTime.Now);

        // Assert
        result.IsSuccess.Should().BeTrue();
        appointment.Status.Should().Be(AppointmentStatusEnum.Completed);
        _mockAppointmentRepository.Verify(x => x.UpdateAsync(appointment), Times.Once);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAppointmentStatusAsync_WithNonExistingAppointment_ReturnsNotFound()
    {
        // Arrange
        var appointmentId = Guid.NewGuid();
        _mockAppointmentRepository.Setup(x => x.GetByIdAsync(appointmentId))
            .ReturnsAsync((Appointment?)null);

        // Act
        var result = await _service.UpdateAppointmentStatusAsync(appointmentId, AppointmentStatusEnum.Completed, DateTime.Now);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorType.Should().Be(ErrorTypeEnum.NotFound);
        result.ErrorMessage.Should().Contain("Appointment not found");
    }

    [Fact]
    public async Task UpdateAppointmentStatusAsync_ToCancelledWithinOneHour_ReturnsBusinessRuleViolation()
    {
        // Arrange
        var appointmentId = Guid.NewGuid();
        var currentTime = DateTime.Now;
        var appointment = new Appointment
        {
            Id = appointmentId,
            AnimalId = Guid.NewGuid(),
            VeterinarianId = Guid.NewGuid(),
            StartTime = currentTime.AddMinutes(30), // 30 minutes from now
            EndTime = currentTime.AddMinutes(90),
            Status = AppointmentStatusEnum.Scheduled
        };

        _mockAppointmentRepository.Setup(x => x.GetByIdAsync(appointmentId))
            .ReturnsAsync(appointment);

        // Act
        var result = await _service.UpdateAppointmentStatusAsync(appointmentId, AppointmentStatusEnum.Cancelled, currentTime);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorType.Should().Be(ErrorTypeEnum.BusinessRuleViolation);
        result.ErrorMessage.Should().Contain("Cannot cancel appointment within 1 hour");
    }

    [Fact]
    public async Task UpdateAppointmentStatusAsync_ToCancelledMoreThanOneHour_SendsNotification()
    {
        // Arrange
        var appointmentId = Guid.NewGuid();
        var currentTime = DateTime.Now;
        var appointment = new Appointment
        {
            Id = appointmentId,
            AnimalId = Guid.NewGuid(),
            VeterinarianId = Guid.NewGuid(),
            StartTime = currentTime.AddHours(2),
            EndTime = currentTime.AddHours(3),
            Status = AppointmentStatusEnum.Scheduled,
            Animal = new Animal 
            { 
                Name = "Fluffy",
                Owner = new Owner 
                { 
                    Name = "Test Owner",
                    Email = "owner@test.com",
                    PhoneNumber = "+1234567890",
                    Address = "123 Test St"
                }
            }
        };

        _mockAppointmentRepository.Setup(x => x.GetByIdAsync(appointmentId))
            .ReturnsAsync(appointment);
        _mockAppointmentRepository.Setup(x => x.UpdateAsync(It.IsAny<Appointment>()))
            .Returns(Task.CompletedTask);
        _mockUnitOfWork.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _service.UpdateAppointmentStatusAsync(appointmentId, AppointmentStatusEnum.Cancelled, currentTime);

        // Assert
        result.IsSuccess.Should().BeTrue();
        appointment.Status.Should().Be(AppointmentStatusEnum.Cancelled);
        
        _mockNotificationService.Verify(x => x.SendAppointmentCancellationEmail(
            "owner@test.com",
            "Fluffy",
            appointment.StartTime), Times.Once);
    }

    [Fact]
    public async Task UpdateAppointmentStatusAsync_WithInProgressStatus_MigratesToScheduled()
    {
        // Arrange
        var appointmentId = Guid.NewGuid();
        var appointment = new Appointment
        {
            Id = appointmentId,
            AnimalId = Guid.NewGuid(),
            VeterinarianId = Guid.NewGuid(),
            StartTime = DateTime.Now.AddHours(2),
            EndTime = DateTime.Now.AddHours(3),
            Status = AppointmentStatusEnum.Scheduled
        };

        _mockAppointmentRepository.Setup(x => x.GetByIdAsync(appointmentId))
            .ReturnsAsync(appointment);
        _mockAppointmentRepository.Setup(x => x.UpdateAsync(It.IsAny<Appointment>()))
            .Returns(Task.CompletedTask);
        _mockUnitOfWork.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        #pragma warning disable CS0618 // Type or member is obsolete
        var result = await _service.UpdateAppointmentStatusAsync(appointmentId, AppointmentStatusEnum.InProgress, DateTime.Now);
        #pragma warning restore CS0618 // Type or member is obsolete

        // Assert
        result.IsSuccess.Should().BeTrue();
        appointment.Status.Should().Be(AppointmentStatusEnum.Scheduled); // Should be migrated
    }

    [Fact]
    public async Task UpdateAppointmentStatusAsync_WithNoShowStatus_ForPastAppointment_ReturnsBusinessRuleViolation()
    {
        // Arrange
        var appointmentId = Guid.NewGuid();
        var appointment = new Appointment
        {
            Id = appointmentId,
            AnimalId = Guid.NewGuid(),
            VeterinarianId = Guid.NewGuid(),
            StartTime = DateTime.Now.AddHours(-2), // Past appointment
            EndTime = DateTime.Now.AddHours(-1),
            Status = AppointmentStatusEnum.Scheduled
        };

        _mockAppointmentRepository.Setup(x => x.GetByIdAsync(appointmentId))
            .ReturnsAsync(appointment);

        // Act
        #pragma warning disable CS0618 // Type or member is obsolete
        var result = await _service.UpdateAppointmentStatusAsync(appointmentId, AppointmentStatusEnum.NoShow, DateTime.Now);
        #pragma warning restore CS0618 

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorType.Should().Be(ErrorTypeEnum.BusinessRuleViolation);
        result.ErrorMessage.Should().Contain("Cannot cancel appointment within 1 hour");
        
        // Status should not have changed
        appointment.Status.Should().Be(AppointmentStatusEnum.Scheduled);
    }

    [Fact]
    public async Task UpdateAppointmentStatusAsync_WhenExceptionOccurs_ReturnsInternalError()
    {
        // Arrange
        var appointmentId = Guid.NewGuid();
        _mockAppointmentRepository.Setup(x => x.GetByIdAsync(appointmentId))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _service.UpdateAppointmentStatusAsync(appointmentId, AppointmentStatusEnum.Completed, DateTime.Now);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorType.Should().Be(ErrorTypeEnum.InternalError);
        result.ErrorMessage.Should().Contain("error occurred while updating appointment status");
    }
}