using MapsterMapper;
using MediatR;
using Portfolio.Application.DTOs;
using Portfolio.Domain.Entities;
using Portfolio.Domain.Interfaces;

namespace Portfolio.Application.Features.Skills.Queries;

public class GetAllSkillsQueryHandler : IRequestHandler<GetAllSkillsQuery, List<SkillDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllSkillsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<SkillDto>> Handle(GetAllSkillsQuery request, CancellationToken cancellationToken)
    {
        var skills = await _unitOfWork.Skills.GetAllAsync(cancellationToken);

        return _mapper.Map<List<SkillDto>>(
            skills
                .OrderBy(s => s.Category)
                .ThenBy(s => s.DisplayOrder)
                .ToList());
    }
}
