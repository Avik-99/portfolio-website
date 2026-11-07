namespace Portfolio.Application.DTOs;

public record AchievementDto(
    Guid Id,
    string Title,
    string Description,
    DateTime AchievementDate,
    int DisplayOrder
);
