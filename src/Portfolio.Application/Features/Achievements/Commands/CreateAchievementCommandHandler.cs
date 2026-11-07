using MediatR;
using Portfolio.Domain.Entities;
using Portfolio.Domain.Interfaces;

namespace Portfolio.Application.Features.Achievements.Commands;

public class CreateAchievementCommandHandler : IRequestHandler<CreateAchievementCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateAchievementCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateAchievementCommand request, CancellationToken cancellationToken)
    {
        var achievement = new Achievement(
            request.Title,
            request.Description,
            request.AchievementDate,
            request.DisplayOrder);

        await _unitOfWork.Achievements.AddAsync(achievement, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return achievement.Id;
    }
}
