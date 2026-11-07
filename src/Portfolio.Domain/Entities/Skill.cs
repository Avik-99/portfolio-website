using Portfolio.Domain.Common;

namespace Portfolio.Domain.Entities;

public enum SkillCategory
{
    BackendAndArchitecture = 1,
    DatabaseAndSecurity = 2,
    Testing = 3,
    DevOpsAndTools = 4,
    Frontend = 5
}

public class Skill : AuditableEntity<Guid>
{
    public string Name { get; private set; } = string.Empty;
    public SkillCategory Category { get; private set; }
    public string? IconClass { get; private set; }
    public int DisplayOrder { get; private set; }

    private Skill() { }

    public Skill(string name, SkillCategory category, string? iconClass, int displayOrder)
    {
        Id = Guid.NewGuid();
        Name = name;
        Category = category;
        IconClass = iconClass;
        DisplayOrder = displayOrder;
    }

    public void UpdateDetails(string name, SkillCategory category, string? iconClass, int displayOrder)
    {
        Name = name;
        Category = category;
        IconClass = iconClass;
        DisplayOrder = displayOrder;
    }
}
