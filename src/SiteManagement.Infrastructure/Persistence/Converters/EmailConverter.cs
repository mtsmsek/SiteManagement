using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using DomainEmail = SiteManagement.Domain.Residency.ValueObjects.Email;

namespace SiteManagement.Infrastructure.Persistence.Converters;

/// <summary>Maps the Residency <c>Email</c> value object to/from its lower-cased string storage form.</summary>
public sealed class EmailConverter : ValueConverter<DomainEmail, string>
{
    public EmailConverter()
        : base(v => v.Value, v => DomainEmail.From(v))
    {
    }
}
