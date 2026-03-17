namespace CO.Shared.Dtos.Common;

/// <summary>
/// Base DTO record that carries the common audit fields present on every entity.
/// All read-model DTOs that represent a persisted entity should inherit from this.
/// </summary>
public abstract record BaseDto(
    Guid Id,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt);
