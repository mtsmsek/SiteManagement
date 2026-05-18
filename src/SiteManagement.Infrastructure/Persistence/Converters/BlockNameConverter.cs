using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SiteManagement.Domain.Property.ValueObjects;

namespace SiteManagement.Infrastructure.Persistence.Converters;

/// <summary>Maps <see cref="BlockName"/> to/from its trimmed display string.</summary>
public sealed class BlockNameConverter : ValueConverter<BlockName, string>
{
    public BlockNameConverter()
        : base(v => v.Value, v => BlockName.From(v))
    {
    }
}
