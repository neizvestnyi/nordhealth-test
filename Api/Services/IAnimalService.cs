using Api.Common;
using Api.DTOs.Requests;
using Api.Models;

namespace Api.Services;

public interface IAnimalService
{
    Task<Result<Animal>> CreateAnimalAsync(CreateAnimalRequest request);
    Task<Result<Animal>> GetAnimalByIdAsync(Guid id);
    Task<Result> DeleteAnimalAsync(Guid id);
}