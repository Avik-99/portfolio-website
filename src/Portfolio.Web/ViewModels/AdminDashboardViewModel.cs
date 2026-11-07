using Portfolio.Application.DTOs;

namespace Portfolio.Web.ViewModels;

public class AdminDashboardViewModel
{
    public IReadOnlyList<ProjectDto> Projects { get; set; } = [];
    public IReadOnlyList<ContactMessageDto> Messages { get; set; } = [];

    public int UnreadMessages => Messages.Count(m => !m.IsRead);
    public int TotalProjects => Projects.Count;
    public int FeaturedProjects => Projects.Count(p => p.IsFeatured);
}
