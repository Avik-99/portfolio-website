using MediatR;
using Portfolio.Application.DTOs;

namespace Portfolio.Application.Features.Skills.Queries;

public record GetAllSkillsQuery : IRequest<List<SkillDto>>;
