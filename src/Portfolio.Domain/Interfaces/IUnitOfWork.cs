using Portfolio.Domain.Entities;

namespace Portfolio.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IRepository<Project> Projects { get; }
    IRepository<Skill> Skills { get; }
    IRepository<ContactMessage> ContactMessages { get; }
    IRepository<Achievement> Achievements { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
