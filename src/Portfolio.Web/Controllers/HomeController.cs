using MediatR;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Application.Features.Achievements.Queries;
using Portfolio.Application.Features.Projects.Queries;
using Portfolio.Application.Features.Skills.Queries;
using Portfolio.Web.ViewModels;

namespace Portfolio.Web.Controllers;

public class HomeController : Controller
{
    private readonly IMediator _mediator;

    public HomeController(IMediator mediator) => _mediator = mediator;

    [HttpGet("/")]
    public async Task<IActionResult> Index()
    {
        var featuredProjects = await _mediator.Send(new GetFeaturedProjectsQuery());
        var skills = await _mediator.Send(new GetAllSkillsQuery());
        var achievements = await _mediator.Send(new GetAllAchievementsQuery());

        var viewModel = new HomeViewModel
        {
            FeaturedProjects = featuredProjects,
            SkillsCategoryGrouped = skills
                .GroupBy(s => s.CategoryName)
                .ToDictionary(g => g.Key, g => g.ToList()),
            Achievements = achievements
        };

        return View(viewModel);
    }

    [HttpGet("home/error")]
    public IActionResult Error()
    {
        return View();
    }
}
