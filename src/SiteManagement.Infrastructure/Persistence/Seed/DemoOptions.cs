namespace SiteManagement.Infrastructure.Persistence.Seed;

/// <summary>
/// Options block for the optional demo data seeder. Bound from the
/// <c>Demo</c> section of configuration; the toggle is off by default so the
/// seeder never runs unless a developer explicitly opts in.
/// </summary>
public sealed class DemoOptions
{
    /// <summary>Configuration section name (<c>Demo</c>).</summary>
    public const string SectionName = "Demo";

    /// <summary>When true, <see cref="DemoSeeder"/> is invoked at startup.</summary>
    public bool SeedOnStartup { get; init; }
}
