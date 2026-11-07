using MapsterMapper;
using MediatR;
using Portfolio.Application.DTOs;
using Portfolio.Domain.Interfaces;

namespace Portfolio.Application.Features.Projects.Queries;

public class GetAllProjectsQueryHandler : IRequestHandler<GetAllProjectsQuery, List<ProjectDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllProjectsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<ProjectDto>> Handle(GetAllProjectsQuery request, CancellationToken cancellationToken)
    {
        var projects = await _unitOfWork.Projects.GetAllAsync(cancellationToken);

        return _mapper.Map<List<ProjectDto>>(
            projects.OrderBy(p => p.DisplayOrder).ToList());
    }
}
