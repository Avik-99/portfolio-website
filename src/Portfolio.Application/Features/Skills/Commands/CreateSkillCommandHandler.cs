using MediatR;
using Portfolio.Domain.Entities;
using Portfolio.Domain.Interfaces;

namespace Portfolio.Application.Features.Skills.Commands;

public class CreateSkillCommandHandler : IRequestHandler<CreateSkillCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateSkillCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateSkillCommand request, CancellationToken cancellationToken)
    {
        var skill = new Skill(request.Name, request.Category, request.IconClass, request.DisplayOrder);

        await _unitOfWork.Skills.AddAsync(skill, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return skill.Id;
    }
}
