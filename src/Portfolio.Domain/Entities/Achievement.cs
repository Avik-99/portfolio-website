using Portfolio.Domain.Common;

namespace Portfolio.Domain.Entities;

public class Achievement : AuditableEntity<Guid>
{
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public DateTime AchievementDate { get; private set; }
    public int DisplayOrder { get; private set; }

    private Achievement() { }

    public Achievement(string title, string description, DateTime achievementDate, int displayOrder)
    {
        Id = Guid.NewGuid();
        Title = title;
        Description = description;
        AchievementDate = achievementDate;
        DisplayOrder = displayOrder;
    }

    public void Update(string title, string description, DateTime achievementDate, int displayOrder)
    {
        Title = title;
        Description = description;
        AchievementDate = achievementDate;
        DisplayOrder = displayOrder;
    }
}
