using Api.Common;
using Api.DTOs.Requests;
using Api.Models;
using Api.Repositories;

namespace Api.Services;

public class AnimalService : IAnimalService
{
    private readonly IUnitOfWork _unitOfWork;

    public AnimalService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Animal>> CreateAnimalAsync(CreateAnimalRequest request)
    {
        try
        {
            if (request == null)
            {
                return Result<Animal>.Failure("Animal request cannot be null.", ErrorTypeEnum.ValidationError);
            }

            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return Result<Animal>.Failure("Animal name is required.", ErrorTypeEnum.ValidationError);
            }

            var animal = new Animal
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                BirthDate = request.BirthDate,
                OwnerId = request.OwnerId
            };

            await _unitOfWork.Animals.AddAsync(animal);
            await _unitOfWork.SaveChangesAsync();

            // Reload with navigation properties
            var createdAnimal = await _unitOfWork.Animals.GetByIdAsync(animal.Id);

            return Result<Animal>.Success(createdAnimal!);
        }
        catch (Exception ex)
        {
            return Result<Animal>.Failure($"An error occurred while creating animal: {ex.Message}", ErrorTypeEnum.InternalError);
        }
    }

    public async Task<Result<Animal>> GetAnimalByIdAsync(Guid id)
    {
        try
        {
            var animal = await _unitOfWork.Animals.GetByIdAsync(id);
            if (animal == null)
            {
                return Result<Animal>.Failure("Animal not found.", ErrorTypeEnum.NotFound);
            }

            return Result<Animal>.Success(animal);
        }
        catch (Exception ex)
        {
            return Result<Animal>.Failure($"An error occurred while retrieving animal: {ex.Message}", ErrorTypeEnum.InternalError);
        }
    }

    public async Task<Result> DeleteAnimalAsync(Guid id)
    {
        try
        {
            var animal = await _unitOfWork.Animals.GetByIdAsync(id);
            if (animal == null)
            {
                return Result.Failure("Animal not found.", ErrorTypeEnum.NotFound);
            }

            await _unitOfWork.Animals.DeleteAsync(animal);
            await _unitOfWork.SaveChangesAsync();

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"An error occurred while deleting animal: {ex.Message}", ErrorTypeEnum.InternalError);
        }
    }
}