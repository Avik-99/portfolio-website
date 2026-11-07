using Microsoft.EntityFrameworkCore;
using Portfolio.Domain.Common;
using Portfolio.Domain.Entities;
using Portfolio.Domain.Interfaces;
using Portfolio.Infrastructure.Persistence;
using Portfolio.Infrastructure.Repositories;

namespace Portfolio.Infrastructure;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
        Projects = new Repository<Project>(context);
        Skills = new Repository<Skill>(context);
        ContactMessages = new Repository<ContactMessage>(context);
        Achievements = new Repository<Achievement>(context);
    }

    public IRepository<Project> Projects { get; }
    public IRepository<Skill> Skills { get; }
    public IRepository<ContactMessage> ContactMessages { get; }
    public IRepository<Achievement> Achievements { get; }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyAuditTimestamps();
        return await _context.SaveChangesAsync(cancellationToken);
    }

    private void ApplyAuditTimestamps()
    {
        foreach (var entry in _context.ChangeTracker.Entries<AuditableEntity<Guid>>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAtUtc = DateTime.UtcNow;
                    entry.Entity.CreatedBy = "System";
                    break;

                case EntityState.Modified:
                    entry.Entity.LastModifiedAtUtc = DateTime.UtcNow;
                    entry.Entity.LastModifiedBy = "System";
                    break;
            }
        }
    }

    public void Dispose() => _context.Dispose();
}
