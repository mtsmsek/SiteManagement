using System.Reflection;

namespace PaymentService.ArchitectureTests;

/// <summary>
/// Single source of truth for the PaymentService assemblies the architecture
/// tests inspect. Each property loads the project assembly via a representative
/// type so the tests stay strongly-typed and survive renames.
/// </summary>
public static class AssemblyReferences
{
    /// <summary>The Domain assembly (must stay framework-free — no Mongo/EF/ASP).</summary>
    public static Assembly Domain { get; } = typeof(PaymentService.Domain.BankAccount).Assembly;

    /// <summary>The Application assembly (use-case orchestration; no persistence/HTTP).</summary>
    public static Assembly Application { get; } = typeof(PaymentService.Application.DependencyInjection).Assembly;

    /// <summary>The Infrastructure assembly (Mongo persistence lives here).</summary>
    public static Assembly Infrastructure { get; } = typeof(PaymentService.Infrastructure.DependencyInjection).Assembly;
}
