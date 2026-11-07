using MediatR;
using Portfolio.Domain.Entities;

namespace Portfolio.Application.Features.Skills.Commands;

public record CreateSkillCommand(
    string Name,
    SkillCategory Category,
    string? IconClass,
    int DisplayOrder
) : IRequest<Guid>;
