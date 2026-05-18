using FluentAssertions;
using SiteManagement.Domain.Residency;
using SiteManagement.Domain.Residency.Exceptions;
using SiteManagement.Domain.Residency.ValueObjects;
using SiteManagement.Domain.Tests.Doubles;

namespace SiteManagement.Domain.Tests.Residency;

/// <summary>
/// Specifies the <see cref="Resident"/> aggregate root: identity, contact
/// info updates, name changes, and the vehicle collection invariants
/// (unique plate; remove-by-plate honours the equality contract).
/// </summary>
public class ResidentTests
{
    [Fact]
    public void Create_AssignsIdAndExposesValues()
    {
        // arrange + act
        var resident = ResidencyDoubles.SampleResident();

        // assert
        resident.Id.Should().NotBe(Guid.Empty);
        resident.TcNo.Value.Should().Be(ResidencyDoubles.SampleTcRaw);
        resident.FullName.FirstName.Should().Be("Ada");
        resident.Vehicles.Should().BeEmpty();
    }

    [Fact]
    public void UpdateContactInfo_ReplacesEmailAndPhone()
    {
        // arrange
        var resident = ResidencyDoubles.SampleResident();
        var newEmail = Email.From("new@example.com");
        var newPhone = PhoneNumber.From("05559876543");

        // act
        resident.UpdateContactInfo(newEmail, newPhone);

        // assert
        resident.Email.Should().Be(newEmail);
        resident.Phone.Should().Be(newPhone);
    }

    [Fact]
    public void ChangeFullName_ReplacesName()
    {
        // arrange
        var resident = ResidencyDoubles.SampleResident();
        var newName = FullName.Create("Grace", "Hopper");

        // act
        resident.ChangeFullName(newName);

        // assert
        resident.FullName.Should().Be(newName);
    }

    [Fact]
    public void AddVehicle_NewPlate_Succeeds()
    {
        // arrange
        var resident = ResidencyDoubles.SampleResident();
        var vehicle = ResidencyDoubles.SampleVehicle();

        // act
        resident.AddVehicle(vehicle);

        // assert
        resident.Vehicles.Should().ContainSingle().Which.Should().Be(vehicle);
    }

    [Fact]
    public void AddVehicle_DuplicatePlate_Throws()
    {
        // arrange
        var resident = ResidencyDoubles.SampleResident();
        resident.AddVehicle(ResidencyDoubles.SampleVehicle("34ABC123"));

        // act — different casing, same plate after normalisation.
        var act = () => resident.AddVehicle(ResidencyDoubles.SampleVehicle("34abc123"));

        // assert
        act.Should().Throw<DuplicateVehiclePlateException>();
    }

    [Fact]
    public void RemoveVehicle_ExistingPlate_Removes()
    {
        // arrange
        var resident = ResidencyDoubles.SampleResident();
        var plate = PlateNumber.From("34ABC123");
        resident.AddVehicle(VehicleInfo.Create(plate));

        // act
        resident.RemoveVehicle(plate);

        // assert
        resident.Vehicles.Should().BeEmpty();
    }

    [Fact]
    public void RemoveVehicle_UnknownPlate_Throws()
    {
        // arrange
        var resident = ResidencyDoubles.SampleResident();
        var unknownPlate = PlateNumber.From("06XYZ789");

        // act
        var act = () => resident.RemoveVehicle(unknownPlate);

        // assert
        act.Should().Throw<VehicleNotFoundException>();
    }

    [Fact]
    public void Vehicles_CollectionIsReadOnly()
    {
        // arrange
        var resident = ResidencyDoubles.SampleResident();

        // assert — exposing as IReadOnlyCollection<VehicleInfo> blocks callers
        // from poking at the backing list and bypassing the duplicate-plate guard.
        resident.Vehicles.Should().BeAssignableTo<IReadOnlyCollection<VehicleInfo>>();
        resident.Vehicles.Should().NotBeAssignableTo<List<VehicleInfo>>();
    }
}
