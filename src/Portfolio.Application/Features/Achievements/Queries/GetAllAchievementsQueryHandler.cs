using MapsterMapper;
using MediatR;
using Portfolio.Application.DTOs;
using Portfolio.Domain.Interfaces;

namespace Portfolio.Application.Features.Achievements.Queries;

public class GetAllAchievementsQueryHandler : IRequestHandler<GetAllAchievementsQuery, List<AchievementDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllAchievementsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<AchievementDto>> Handle(GetAllAchievementsQuery request, CancellationToken cancellationToken)
    {
        var achievements = await _unitOfWork.Achievements.GetAllAsync(cancellationToken);

        return _mapper.Map<List<AchievementDto>>(
            achievements.OrderBy(a => a.DisplayOrder).ToList());
    }
}
