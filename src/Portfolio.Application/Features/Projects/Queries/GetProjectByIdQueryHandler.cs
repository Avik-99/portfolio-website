using MapsterMapper;
using MediatR;
using Portfolio.Application.DTOs;
using Portfolio.Domain.Interfaces;

namespace Portfolio.Application.Features.Projects.Queries;

public class GetProjectByIdQueryHandler : IRequestHandler<GetProjectByIdQuery, ProjectDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetProjectByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ProjectDto?> Handle(GetProjectByIdQuery request, CancellationToken cancellationToken)
    {
        var project = await _unitOfWork.Projects.GetByIdAsync<Guid>(request.Id, cancellationToken);

        return project is null ? null : _mapper.Map<ProjectDto>(project);
    }
}
