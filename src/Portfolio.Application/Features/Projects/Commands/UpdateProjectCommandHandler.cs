using MediatR;
using Portfolio.Domain.Interfaces;

namespace Portfolio.Application.Features.Projects.Commands;

public class UpdateProjectCommandHandler : IRequestHandler<UpdateProjectCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProjectCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
    {
        var project = await _unitOfWork.Projects.GetByIdAsync<Guid>(request.Id, cancellationToken);

        if (project is null)
        {
            throw new KeyNotFoundException($"Project with ID '{request.Id}' was not found.");
        }

        project.Update(
            request.Title,
            request.Description,
            request.Category,
            request.LiveDemoUrl,
            request.GithubUrl,
            request.ImageUrl,
            request.IsFeatured,
            request.DisplayOrder,
            request.TechnicalHighlights,
            request.TechStack);

        _unitOfWork.Projects.Update(project);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
