using MediatR;

namespace Portfolio.Application.Features.Skills.Commands;

public record DeleteSkillCommand(Guid Id) : IRequest<bool>;
