using System.Reflection;

namespace SiteManagement.ArchitectureTests;

/// <summary>
/// Single source of truth for the assemblies the architecture tests inspect.
/// Each property loads the project assembly via a representative type so the
/// tests stay strongly-typed and survive renames.
/// </summary>
public static class AssemblyReferences
{
    /// <summary>The Domain assembly (must stay framework-free).</summary>
    public static Assembly Domain { get; } = typeof(SiteManagement.Domain.Identity.Roles).Assembly;

    /// <summary>The Application assembly (must stay framework-agnostic).</summary>
    public static Assembly Application { get; } = typeof(SiteManagement.Application.DependencyInjection).Assembly;

    /// <summary>The Infrastructure assembly (may use EF Core / Identity / JWT internals).</summary>
    public static Assembly Infrastructure { get; } = typeof(SiteManagement.Infrastructure.DependencyInjection).Assembly;

    /// <summary>The Api assembly (the only place ASP.NET Core hosting + Scalar live).</summary>
    public static Assembly Api { get; } = typeof(Program).Assembly;
}
