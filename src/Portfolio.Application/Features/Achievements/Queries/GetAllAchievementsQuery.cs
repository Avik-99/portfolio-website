using MediatR;
using Portfolio.Application.DTOs;

namespace Portfolio.Application.Features.Achievements.Queries;

public record GetAllAchievementsQuery : IRequest<List<AchievementDto>>;
