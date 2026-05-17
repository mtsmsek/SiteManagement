namespace SiteManagement.Application.Shared.Exceptions;

/// <summary>
/// Thrown when a command/query references an aggregate that does not exist.
/// Renders as HTTP 404.
/// </summary>
public sealed class EntityNotFoundException : ApplicationException
{
    /// <summary>Creates an exception describing the missing entity by name and key.</summary>
    /// <param name="entityName">Aggregate type name (e.g. <c>"Apartment"</c>).</param>
    /// <param name="key">Lookup key that produced no result.</param>
    public EntityNotFoundException(string entityName, object key)
        : base($"{entityName} with key '{key}' was not found.", HttpStatus.NotFound)
    {
        EntityName = entityName;
        Key = key;
    }

    /// <summary>Type name of the missing aggregate.</summary>
    public string EntityName { get; }

    /// <summary>Lookup key that produced no result.</summary>
    public object Key { get; }
}
