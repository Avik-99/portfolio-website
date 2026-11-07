using MapsterMapper;
using MediatR;
using Portfolio.Application.DTOs;
using Portfolio.Domain.Interfaces;

namespace Portfolio.Application.Features.Projects.Queries;

public class GetFeaturedProjectsQueryHandler : IRequestHandler<GetFeaturedProjectsQuery, List<ProjectDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetFeaturedProjectsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<ProjectDto>> Handle(GetFeaturedProjectsQuery request, CancellationToken cancellationToken)
    {
        var featuredProjects = await _unitOfWork.Projects.FindAsync(p => p.IsFeatured, cancellationToken);

        return _mapper.Map<List<ProjectDto>>(
            featuredProjects.OrderBy(p => p.DisplayOrder).ToList());
    }
}
