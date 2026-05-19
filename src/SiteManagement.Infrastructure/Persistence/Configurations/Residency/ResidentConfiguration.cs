using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SiteManagement.Domain.Residency;
using SiteManagement.Infrastructure.Persistence.Converters;

namespace SiteManagement.Infrastructure.Persistence.Configurations.Residency;

/// <summary>
/// EF mapping for the <see cref="Resident"/> aggregate root. TcNo and Email
/// get unique indexes so duplicates fail at the database boundary, the
/// FullName value object is flattened into FirstName/LastName columns via
/// EF's complex-property support, and <c>Vehicles</c> becomes an owned-
/// collection child table.
/// </summary>
public sealed class ResidentConfiguration : IEntityTypeConfiguration<Resident>
{
    public void Configure(EntityTypeBuilder<Resident> builder)
    {
        builder.ToTable(SchemaConstants.Tables.Residents);

        builder.HasKey(r => r.Id);

        // Same reason as SiteConfiguration: lets EF skip the empty UPDATE
        // when AddVehicle / RemoveVehicle flips the owned collection
        // without touching any scalar resident column.
        builder.Property<uint>(SchemaConstants.ConcurrencyTokenColumn)
            .HasColumnName(SchemaConstants.ConcurrencyTokenColumn)
            .IsConcurrencyToken()
            .ValueGeneratedOnAddOrUpdate();

        builder.Property(r => r.TcNo)
            .HasConversion(new TcNoConverter())
            .HasMaxLength(ResidencyLimits.TcNoLength)
            .IsRequired();

        builder.HasIndex(r => r.TcNo).IsUnique();

        builder.ComplexProperty(r => r.FullName, fn =>
        {
            fn.Property(p => p.FirstName)
                .HasMaxLength(ResidencyLimits.NameComponentMaxLength)
                .IsRequired();
            fn.Property(p => p.LastName)
                .HasMaxLength(ResidencyLimits.NameComponentMaxLength)
                .IsRequired();
        });

        builder.Property(r => r.Email)
            .HasConversion(new EmailConverter())
            .HasMaxLength(ResidencyLimits.EmailMaxLength)
            .IsRequired();

        builder.HasIndex(r => r.Email).IsUnique();

        builder.Property(r => r.Phone)
            .HasConversion(new PhoneNumberConverter())
            .HasMaxLength(ResidencyLimits.PhoneNumberMaxLength)
            .IsRequired();

        builder.OwnsMany(r => r.Vehicles, vehicles =>
        {
            vehicles.ToTable(SchemaConstants.Tables.ResidentVehicles);

            vehicles.WithOwner().HasForeignKey(SchemaConstants.ForeignKeys.ResidentId);
            vehicles.Property<int>(SchemaConstants.OwnedSurrogateKey);
            vehicles.HasKey(SchemaConstants.OwnedSurrogateKey);

            vehicles.Property(v => v.Plate)
                .HasConversion(new PlateNumberConverter())
                .HasMaxLength(ResidencyLimits.PlateNumberMaxLength)
                .IsRequired();

            vehicles.Property(v => v.Note)
                .HasMaxLength(ResidencyLimits.VehicleNoteMaxLength);

            vehicles.HasIndex(SchemaConstants.ForeignKeys.ResidentId, nameof(Domain.Residency.ValueObjects.VehicleInfo.Plate))
                .IsUnique();
        });

        builder.Navigation(r => r.Vehicles)
            .HasField(SchemaConstants.BackingFields.ResidentVehicles)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Ignore(r => r.DomainEvents);
    }
}
