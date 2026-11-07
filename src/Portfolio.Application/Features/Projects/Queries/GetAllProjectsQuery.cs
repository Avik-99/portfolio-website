using MediatR;
using Portfolio.Application.DTOs;

namespace Portfolio.Application.Features.Projects.Queries;

public record GetAllProjectsQuery : IRequest<List<ProjectDto>>;
