using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SiteManagement.Domain.Residency.ValueObjects;

namespace SiteManagement.Infrastructure.Persistence.Converters;

/// <summary>Maps <see cref="Email"/> to/from its lower-cased string storage form.</summary>
public sealed class EmailConverter : ValueConverter<Email, string>
{
    public EmailConverter()
        : base(v => v.Value, v => Email.From(v))
    {
    }
}
