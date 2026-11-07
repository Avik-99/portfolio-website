using MediatR;

namespace Portfolio.Application.Features.Projects.Commands;

public record DeleteProjectCommand(Guid Id) : IRequest<bool>;
