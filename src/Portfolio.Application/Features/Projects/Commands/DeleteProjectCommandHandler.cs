using MediatR;
using Portfolio.Domain.Interfaces;

namespace Portfolio.Application.Features.Projects.Commands;

public class DeleteProjectCommandHandler : IRequestHandler<DeleteProjectCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteProjectCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
    {
        var project = await _unitOfWork.Projects.GetByIdAsync<Guid>(request.Id, cancellationToken);

        if (project is null)
        {
            throw new KeyNotFoundException($"Project with ID '{request.Id}' was not found.");
        }

        _unitOfWork.Projects.Remove(project);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
