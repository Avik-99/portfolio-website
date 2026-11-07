using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Portfolio.Infrastructure.Persistence;

namespace Portfolio.Infrastructure.Identity;

/// <summary>
/// Applies EF Core migrations and seeds the default Admin role and
/// initial administrator account on application start.
/// The admin password is never hardcoded: it is read from configuration
/// (env var PORTFOLIO_ADMIN_PASSWORD, or IdentitySeed:AdminPassword).
/// </summary>
public class DbInitializer
{
    private const string AdminRoleName = "Admin";
    private const string DefaultAdminEmail = "admin@portfolio.local";

    private readonly ApplicationDbContext _context;
    private readonly RoleManager<Microsoft.AspNetCore.Identity.IdentityRole> _roleManager;
    private readonly UserManager<Microsoft.AspNetCore.Identity.IdentityUser> _userManager;
    private readonly ILogger<DbInitializer> _logger;
    private readonly string _adminEmail;
    private readonly string? _adminPassword;

    public DbInitializer(
        ApplicationDbContext context,
        RoleManager<Microsoft.AspNetCore.Identity.IdentityRole> roleManager,
        UserManager<Microsoft.AspNetCore.Identity.IdentityUser> userManager,
        ILogger<DbInitializer> logger,
        IConfiguration configuration)
    {
        _context = context;
        _roleManager = roleManager;
        _userManager = userManager;
        _logger = logger;
        _adminEmail = configuration["IdentitySeed:AdminEmail"] ?? DefaultAdminEmail;
        _adminPassword = configuration["PORTFOLIO_ADMIN_PASSWORD"] ?? configuration["IdentitySeed:AdminPassword"];
    }

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Applying EF Core migrations to database '{Database}'...", _context.Database.GetDbConnection().Database);
        await _context.Database.MigrateAsync(cancellationToken);
        _logger.LogInformation("Migrations applied successfully.");

        await SeedAsync(cancellationToken);
    }

    private async Task SeedAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        await EnsureAdminRoleAsync();
        await EnsureAdminUserAsync();
    }

    private async Task EnsureAdminRoleAsync()
    {
        if (await _roleManager.RoleExistsAsync(AdminRoleName))
        {
            return;
        }

        var roleResult = await _roleManager.CreateAsync(new Microsoft.AspNetCore.Identity.IdentityRole(AdminRoleName));
        if (!roleResult.Succeeded)
        {
            var errors = string.Join("; ", roleResult.Errors.Select(e => e.Description));
            _logger.LogError("Failed to create Admin role: {Errors}", errors);
            throw new InvalidOperationException($"Failed to create the '{AdminRoleName}' role: {errors}");
        }

        _logger.LogInformation("Created role '{Role}'.", AdminRoleName);
    }

    private async Task EnsureAdminUserAsync()
    {
        if (await _userManager.FindByEmailAsync(_adminEmail) is not null)
        {
            _logger.LogInformation("Admin user '{Email}' already exists.", _adminEmail);
            return;
        }

        if (string.IsNullOrWhiteSpace(_adminPassword))
        {
            _logger.LogWarning(
                "Admin seed password not configured (set PORTFOLIO_ADMIN_PASSWORD or IdentitySeed:AdminPassword). Skipping creation of '{Email}'.",
                _adminEmail);
            return;
        }

        var admin = new Microsoft.AspNetCore.Identity.IdentityUser
        {
            UserName = _adminEmail,
            Email = _adminEmail,
            EmailConfirmed = true
        };

        var userResult = await _userManager.CreateAsync(admin, _adminPassword);
        if (!userResult.Succeeded)
        {
            var errors = string.Join("; ", userResult.Errors.Select(e => e.Description));
            _logger.LogError("Failed to create admin user '{Email}': {Errors}", _adminEmail, errors);
            throw new InvalidOperationException($"Failed to create the admin user '{_adminEmail}': {errors}");
        }

        var roleResult = await _userManager.AddToRoleAsync(admin, AdminRoleName);
        if (!roleResult.Succeeded)
        {
            throw new InvalidOperationException(
                $"Admin user '{_adminEmail}' was created but could not be added to the '{AdminRoleName}' role.");
        }

        _logger.LogInformation("Created admin user '{Email}' with role '{Role}'.", _adminEmail, AdminRoleName);
    }
}
