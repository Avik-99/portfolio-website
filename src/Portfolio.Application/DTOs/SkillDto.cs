namespace Portfolio.Application.DTOs;

public record SkillDto(
    Guid Id,
    string Name,
    string CategoryName,
    int CategoryId,
    string? IconClass,
    int DisplayOrder
);
