using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Application.Features.Achievements.Commands;
using Portfolio.Application.Features.Achievements.Queries;
using Portfolio.Application.Features.Contact.Queries;
using Portfolio.Application.Features.Projects.Commands;
using Portfolio.Application.Features.Projects.Queries;
using Portfolio.Application.Features.Skills.Commands;
using Portfolio.Application.Features.Skills.Queries;
using Portfolio.Web.ViewModels;

namespace Portfolio.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
[Route("admin")]
public class AdminController : Controller
{
    private readonly IMediator _mediator;

    public AdminController(IMediator mediator) => _mediator = mediator;

    [HttpGet("dashboard")]
    public async Task<IActionResult> Dashboard()
    {
        var projects = await _mediator.Send(new GetAllProjectsQuery());
        var messages = await _mediator.Send(new GetAllContactMessagesQuery());

        return View(new AdminDashboardViewModel { Projects = projects, Messages = messages });
    }

    [HttpGet("projects")]
    public async Task<IActionResult> Projects()
    {
        var projects = await _mediator.Send(new GetAllProjectsQuery());
        return View(projects);
    }

    [HttpGet("projects/create")]
    public IActionResult CreateProject()
    {
        return View("ProjectForm", new ProjectFormViewModel());
    }

    [HttpPost("projects/create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateProject(ProjectFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View("ProjectForm", model);
        }

        var command = new CreateProjectCommand(
            model.Title,
            model.Description,
            model.Category,
            model.LiveDemoUrl,
            model.GithubUrl,
            model.ImageUrl,
            model.IsFeatured,
            model.DisplayOrder,
            ProjectFormViewModel.ParseLines(model.TechnicalHighlightsText),
            ProjectFormViewModel.ParseLines(model.TechStackText));

        await _mediator.Send(command);

        TempData["SuccessMessage"] = "Project created successfully!";
        return RedirectToAction(nameof(Projects));
    }

    [HttpGet("projects/edit/{id:guid}")]
    public async Task<IActionResult> EditProject(Guid id)
    {
        var project = await _mediator.Send(new GetProjectByIdQuery(id));
        if (project is null)
        {
            TempData["ErrorMessage"] = $"Project '{id}' was not found.";
            return RedirectToAction(nameof(Projects));
        }

        var model = project.Adapt<ProjectFormViewModel>();
        model.TechnicalHighlightsText = string.Join(Environment.NewLine, project.TechnicalHighlights);
        model.TechStackText = string.Join(Environment.NewLine, project.TechStack);

        return View("ProjectForm", model);
    }

    [HttpPost("projects/edit/{id:guid}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditProject(Guid id, ProjectFormViewModel model)
    {
        model.Id = id;

        if (!ModelState.IsValid)
        {
            return View("ProjectForm", model);
        }

        var command = new UpdateProjectCommand(
            id,
            model.Title,
            model.Description,
            model.Category,
            model.LiveDemoUrl,
            model.GithubUrl,
            model.ImageUrl,
            model.IsFeatured,
            model.DisplayOrder,
            ProjectFormViewModel.ParseLines(model.TechnicalHighlightsText),
            ProjectFormViewModel.ParseLines(model.TechStackText));

        await _mediator.Send(command);

        TempData["SuccessMessage"] = "Project updated successfully!";
        return RedirectToAction(nameof(Projects));
    }

    [HttpPost("projects/delete/{id:guid}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteProject(Guid id)
    {
        await _mediator.Send(new DeleteProjectCommand(id));

        TempData["SuccessMessage"] = "Project deleted successfully!";
        return RedirectToAction(nameof(Projects));
    }

    [HttpGet("skills")]
    public async Task<IActionResult> Skills()
    {
        var skills = await _mediator.Send(new GetAllSkillsQuery());
        return View(skills);
    }

    [HttpPost("skills/create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateSkill(SkillFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            TempData["ErrorMessage"] = "Skill data is invalid.";
            return RedirectToAction(nameof(Skills));
        }

        await _mediator.Send(new CreateSkillCommand(model.Name, model.Category, model.IconClass, model.DisplayOrder));

        TempData["SuccessMessage"] = "Skill created successfully!";
        return RedirectToAction(nameof(Skills));
    }

    [HttpPost("skills/delete/{id:guid}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteSkill(Guid id)
    {
        await _mediator.Send(new DeleteSkillCommand(id));

        TempData["SuccessMessage"] = "Skill deleted successfully!";
        return RedirectToAction(nameof(Skills));
    }

    [HttpGet("achievements")]
    public async Task<IActionResult> Achievements()
    {
        var achievements = await _mediator.Send(new GetAllAchievementsQuery());
        return View(achievements);
    }

    [HttpPost("achievements/create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateAchievement(AchievementFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            TempData["ErrorMessage"] = "Achievement data is invalid.";
            return RedirectToAction(nameof(Achievements));
        }

        await _mediator.Send(new CreateAchievementCommand(
            model.Title, model.Description, model.AchievementDate, model.DisplayOrder));

        TempData["SuccessMessage"] = "Achievement created successfully!";
        return RedirectToAction(nameof(Achievements));
    }

    [HttpGet("messages")]
    public async Task<IActionResult> Messages()
    {
        var messages = await _mediator.Send(new GetAllContactMessagesQuery());
        return View(messages);
    }
}
