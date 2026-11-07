using MediatR;
using Portfolio.Domain.Interfaces;

namespace Portfolio.Application.Features.Skills.Commands;

public class DeleteSkillCommandHandler : IRequestHandler<DeleteSkillCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteSkillCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeleteSkillCommand request, CancellationToken cancellationToken)
    {
        var skill = await _unitOfWork.Skills.GetByIdAsync<Guid>(request.Id, cancellationToken);

        if (skill is null)
        {
            throw new KeyNotFoundException($"Skill with ID '{request.Id}' was not found.");
        }

        _unitOfWork.Skills.Remove(skill);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
