using Api.Common;
using Api.Data;
using Api.DTOs.Requests;
using Api.Helpers;
using Api.Models;
using Api.Repositories;
using Api.Services;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace Api.Tests.Services;

public class AnimalServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IAnimalRepository> _mockAnimalRepository;
    private readonly Mock<IAppointmentRepository> _mockAppointmentRepository;
    private readonly Mock<ILogger<AnimalService>> _mockLogger;
    private readonly AnimalService _service;

    public AnimalServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockAnimalRepository = new Mock<IAnimalRepository>();
        _mockAppointmentRepository = new Mock<IAppointmentRepository>();
        _mockLogger = new Mock<ILogger<AnimalService>>();
        
        _mockUnitOfWork.Setup(x => x.Animals).Returns(_mockAnimalRepository.Object);
        _mockUnitOfWork.Setup(x => x.Appointments).Returns(_mockAppointmentRepository.Object);
        
        _service = new AnimalService(_mockUnitOfWork.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task CreateAnimalAsync_WithValidRequest_ReturnsSuccess()
    {
        // Arrange
        var request = new CreateAnimalRequest
        {
            Name = "Fluffy",
            BirthDate = new DateTime(2020, 5, 15),
            OwnerId = Guid.NewGuid()
        };

        var createdAnimal = new Animal
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            BirthDate = request.BirthDate,
            OwnerId = request.OwnerId,
            Owner = new Owner
            {
                Id = request.OwnerId,
                Name = "Test Owner",
                Email = "owner@test.com"
            }
        };

        _mockAnimalRepository.Setup(x => x.AddAsync(It.IsAny<Animal>()))
            .ReturnsAsync((Animal a) => a);
        _mockUnitOfWork.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);
        _mockAnimalRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(createdAnimal);

        // Act
        var result = await _service.CreateAnimalAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Name.Should().Be(request.Name);
        result.Data.BirthDate.Should().Be(request.BirthDate);
        result.Data.OwnerId.Should().Be(request.OwnerId);
        
        _mockAnimalRepository.Verify(x => x.AddAsync(It.IsAny<Animal>()), Times.Once);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateAnimalAsync_WithNullRequest_ReturnsValidationError()
    {
        // Act
        var result = await _service.CreateAnimalAsync(null!);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorType.Should().Be(ErrorTypeEnum.ValidationError);
        result.ErrorMessage.Should().Contain("cannot be null");
        
        _mockAnimalRepository.Verify(x => x.AddAsync(It.IsAny<Animal>()), Times.Never);
    }

    [Fact]
    public async Task CreateAnimalAsync_WithEmptyName_ReturnsValidationError()
    {
        // Arrange
        var request = new CreateAnimalRequest
        {
            Name = "",
            BirthDate = new DateTime(2020, 5, 15),
            OwnerId = Guid.NewGuid()
        };

        // Act
        var result = await _service.CreateAnimalAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorType.Should().Be(ErrorTypeEnum.ValidationError);
        result.ErrorMessage.Should().Contain("Animal name is required");
    }

    [Fact]
    public async Task CreateAnimalAsync_WhenExceptionOccurs_ReturnsInternalError()
    {
        // Arrange
        var request = new CreateAnimalRequest
        {
            Name = "Fluffy",
            BirthDate = new DateTime(2020, 5, 15),
            OwnerId = Guid.NewGuid()
        };

        _mockAnimalRepository.Setup(x => x.AddAsync(It.IsAny<Animal>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _service.CreateAnimalAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorType.Should().Be(ErrorTypeEnum.InternalError);
        result.ErrorMessage.Should().Contain("error occurred while creating animal");
    }

    [Fact]
    public async Task GetAnimalByIdAsync_WithExistingId_ReturnsSuccess()
    {
        // Arrange
        var animalId = Guid.NewGuid();
        var animal = new Animal
        {
            Id = animalId,
            Name = "Fluffy",
            BirthDate = new DateTime(2020, 5, 15),
            OwnerId = Guid.NewGuid(),
            Owner = new Owner { Name = "Test Owner", Email = "owner@test.com", PhoneNumber = "+1234567890", Address = "123 Main St" }
        };

        _mockAnimalRepository.Setup(x => x.GetByIdAsync(animalId))
            .ReturnsAsync(animal);

        // Act
        var result = await _service.GetAnimalByIdAsync(animalId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Id.Should().Be(animalId);
        result.Data.Name.Should().Be("Fluffy");
    }

    [Fact]
    public async Task GetAnimalByIdAsync_WithNonExistingId_ReturnsNotFound()
    {
        // Arrange
        var animalId = Guid.NewGuid();
        _mockAnimalRepository.Setup(x => x.GetByIdAsync(animalId))
            .ReturnsAsync((Animal?)null);

        // Act
        var result = await _service.GetAnimalByIdAsync(animalId);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorType.Should().Be(ErrorTypeEnum.NotFound);
        result.ErrorMessage.Should().Contain("Animal not found");
    }

    [Fact]
    public async Task GetAnimalByIdAsync_WhenExceptionOccurs_ReturnsInternalError()
    {
        // Arrange
        var animalId = Guid.NewGuid();
        _mockAnimalRepository.Setup(x => x.GetByIdAsync(animalId))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _service.GetAnimalByIdAsync(animalId);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorType.Should().Be(ErrorTypeEnum.InternalError);
        result.ErrorMessage.Should().Contain("error occurred while retrieving animal");
    }

    [Fact]
    public async Task DeleteAnimalAsync_WithExistingAnimal_ReturnsSuccess()
    {
        // Arrange
        var animalId = Guid.NewGuid();
        var animal = new Animal
        {
            Id = animalId,
            Name = "Fluffy"
        };

        _mockAnimalRepository.Setup(x => x.GetByIdAsync(animalId))
            .ReturnsAsync(animal);
        _mockAnimalRepository.Setup(x => x.DeleteAsync(animal))
            .Returns(Task.CompletedTask);
        _mockUnitOfWork.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _service.DeleteAnimalAsync(animalId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _mockAnimalRepository.Verify(x => x.DeleteAsync(animal), Times.Once);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteAnimalAsync_WithNonExistingAnimal_ReturnsNotFound()
    {
        // Arrange
        var animalId = Guid.NewGuid();
        _mockAnimalRepository.Setup(x => x.GetByIdAsync(animalId))
            .ReturnsAsync((Animal?)null);

        // Act
        var result = await _service.DeleteAnimalAsync(animalId);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorType.Should().Be(ErrorTypeEnum.NotFound);
        result.ErrorMessage.Should().Contain("Animal not found");
        
        _mockAnimalRepository.Verify(x => x.DeleteAsync(It.IsAny<Animal>()), Times.Never);
    }


    [Fact]
    public async Task DeleteAnimalAsync_WhenExceptionOccurs_ReturnsInternalError()
    {
        // Arrange
        var animalId = Guid.NewGuid();
        _mockAnimalRepository.Setup(x => x.GetByIdAsync(animalId))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _service.DeleteAnimalAsync(animalId);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorType.Should().Be(ErrorTypeEnum.InternalError);
        result.ErrorMessage.Should().Contain("error occurred while deleting animal");
    }
}