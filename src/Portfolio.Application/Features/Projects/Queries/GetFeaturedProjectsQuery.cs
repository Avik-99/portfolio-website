using MediatR;
using Portfolio.Application.DTOs;

namespace Portfolio.Application.Features.Projects.Queries;

public record GetFeaturedProjectsQuery : IRequest<List<ProjectDto>>;
