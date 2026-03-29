namespace NokPizza.Server.Infrastructure.Dto;

/// <summary>
/// DTO representing a dietary constraint with its ID and name.
/// </summary>
/// <param name="Id">The ID of the dietary constraint.</param>
/// <param name="Name">The name of the dietary constraint.</param>
public record DietaryConstraintResponseDto(int Id, string Name);
