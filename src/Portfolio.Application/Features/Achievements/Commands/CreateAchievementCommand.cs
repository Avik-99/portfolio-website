using MediatR;
using Portfolio.Domain.Entities;

namespace Portfolio.Application.Features.Achievements.Commands;

public record CreateAchievementCommand(
    string Title,
    string Description,
    DateTime AchievementDate,
    int DisplayOrder
) : IRequest<Guid>;
