using FluentAssertions;
using SiteManagement.Domain.Tenancy;
using SiteManagement.Domain.Tenancy.Events;
using SiteManagement.Domain.Tenancy.Exceptions;

namespace SiteManagement.Domain.Tests.Tenancy;

/// <summary>
/// Specifies the <see cref="ApartmentAssignment"/> aggregate: linking a
/// resident to an apartment for a period, ending it on move-out, and the
/// domain events that keep Property occupancy in sync. Cross-aggregate links
/// are by id only (no object references).
/// </summary>
public class ApartmentAssignmentTests
{
    private static readonly Guid ApartmentId = Guid.NewGuid();
    private static readonly Guid ResidentId = Guid.NewGuid();
    private static readonly DateOnly Start = new(2026, 1, 1);

    [Fact]
    public void Assign_SetsFieldsActiveAndRaisesEvent()
    {
        // act
        var assignment = ApartmentAssignment.Assign(ApartmentId, ResidentId, TenantType.Tenant, Start);

        // assert
        assignment.ApartmentId.Should().Be(ApartmentId);
        assignment.ResidentId.Should().Be(ResidentId);
        assignment.TenantType.Should().Be(TenantType.Tenant);
        assignment.Period.StartDate.Should().Be(Start);
        assignment.IsActive.Should().BeTrue();

        assignment.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<ResidentAssignedToApartment>()
            .Which.Should().Match<ResidentAssignedToApartment>(e =>
                e.AssignmentId == assignment.Id
                && e.ApartmentId == ApartmentId
                && e.ResidentId == ResidentId);
    }

    [Fact]
    public void End_ClosesPeriodAndRaisesMovedOutEvent()
    {
        // arrange
        var assignment = ApartmentAssignment.Assign(ApartmentId, ResidentId, TenantType.Owner, Start);
        assignment.ClearDomainEvents();
        var moveOut = Start.AddMonths(8);

        // act
        assignment.End(moveOut);

        // assert
        assignment.IsActive.Should().BeFalse();
        assignment.Period.EndDate.Should().Be(moveOut);

        assignment.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<ResidentMovedOut>()
            .Which.ApartmentId.Should().Be(ApartmentId);
    }

    [Fact]
    public void End_AlreadyEnded_Throws()
    {
        // arrange
        var assignment = ApartmentAssignment.Assign(ApartmentId, ResidentId, TenantType.Owner, Start);
        assignment.End(Start.AddMonths(1));

        // act
        var act = () => assignment.End(Start.AddMonths(2));

        // assert
        act.Should().Throw<AssignmentAlreadyEndedException>();
    }

    [Fact]
    public void End_BeforeStart_Throws()
    {
        // arrange
        var assignment = ApartmentAssignment.Assign(ApartmentId, ResidentId, TenantType.Tenant, Start);

        // act
        var act = () => assignment.End(Start.AddDays(-1));

        // assert
        act.Should().Throw<InvalidAssignmentPeriodException>();
    }
}
