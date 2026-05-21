using FluentAssertions;
using SiteManagement.Domain.Tenancy.Exceptions;
using SiteManagement.Domain.Tenancy.ValueObjects;

namespace SiteManagement.Domain.Tests.Tenancy.ValueObjects;

/// <summary>
/// Specifies the <see cref="AssignmentPeriod"/> value object: an open-ended or
/// closed date range for an apartment assignment. Open (no end) means active;
/// the end date may never precede the start.
/// </summary>
public class AssignmentPeriodTests
{
    private static readonly DateOnly Start = new(2026, 1, 1);

    [Fact]
    public void Open_ExposesStartAndNoEnd_IsActive()
    {
        // act
        var period = AssignmentPeriod.Open(Start);

        // assert
        period.StartDate.Should().Be(Start);
        period.EndDate.Should().BeNull();
        period.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Close_SetsEnd_NoLongerActive()
    {
        // arrange
        var period = AssignmentPeriod.Open(Start);
        var end = Start.AddMonths(6);

        // act
        var closed = period.CloseOn(end);

        // assert
        closed.StartDate.Should().Be(Start);
        closed.EndDate.Should().Be(end);
        closed.IsActive.Should().BeFalse();
    }

    [Fact]
    public void Close_OnStartDate_IsAllowed()
    {
        // arrange — a same-day move-out is valid (end == start)
        var period = AssignmentPeriod.Open(Start);

        // act
        var closed = period.CloseOn(Start);

        // assert
        closed.EndDate.Should().Be(Start);
    }

    [Fact]
    public void Close_BeforeStart_Throws()
    {
        // arrange
        var period = AssignmentPeriod.Open(Start);

        // act
        var act = () => period.CloseOn(Start.AddDays(-1));

        // assert
        act.Should().Throw<InvalidAssignmentPeriodException>();
    }

    [Fact]
    public void Equality_SameRange_AreEqual()
    {
        // assert — value semantics
        AssignmentPeriod.Open(Start).Should().Be(AssignmentPeriod.Open(Start));
        AssignmentPeriod.Open(Start).CloseOn(Start.AddMonths(1))
            .Should().Be(AssignmentPeriod.Open(Start).CloseOn(Start.AddMonths(1)));
    }
}
