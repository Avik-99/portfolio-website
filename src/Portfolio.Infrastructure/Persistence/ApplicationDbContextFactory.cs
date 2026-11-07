using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Portfolio.Infrastructure.Persistence;

/// <summary>
/// Design-time factory so `dotnet ef migrations add` works inside the class library
/// without requiring a running host from Portfolio.Web.
/// Uses the documented default connection string only at design time;
/// runtime always reads the connection string from configuration.
/// </summary>
public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlServer("Server=Avik-99\\SQLEXPRESS;Database=PortfolioDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;")
            .Options;

        return new ApplicationDbContext(options);
    }
}
