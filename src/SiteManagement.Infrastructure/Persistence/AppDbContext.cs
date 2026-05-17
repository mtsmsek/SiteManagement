using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SiteManagement.Infrastructure.Identity;

namespace SiteManagement.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options)
    : IdentityDbContext<AppUser, AppRole, Guid>(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Shorter, conventional table names instead of AspNet* prefixes.
        modelBuilder.Entity<AppUser>(b => b.ToTable("Users"));
        modelBuilder.Entity<AppRole>(b => b.ToTable("Roles"));
        modelBuilder.Entity<Microsoft.AspNetCore.Identity.IdentityUserRole<Guid>>(b => b.ToTable("UserRoles"));
        modelBuilder.Entity<Microsoft.AspNetCore.Identity.IdentityUserClaim<Guid>>(b => b.ToTable("UserClaims"));
        modelBuilder.Entity<Microsoft.AspNetCore.Identity.IdentityUserLogin<Guid>>(b => b.ToTable("UserLogins"));
        modelBuilder.Entity<Microsoft.AspNetCore.Identity.IdentityRoleClaim<Guid>>(b => b.ToTable("RoleClaims"));
        modelBuilder.Entity<Microsoft.AspNetCore.Identity.IdentityUserToken<Guid>>(b => b.ToTable("UserTokens"));
    }
}
