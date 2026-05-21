using SiteManagement.Domain.Shared;
using SiteManagement.Domain.Tenancy.Exceptions;

namespace SiteManagement.Domain.Tenancy.ValueObjects;

/// <summary>
/// The date range an apartment assignment is in effect. Open-ended (no end
/// date) means the assignment is currently active; closing it records the
/// move-out date, which may never precede the start. Modelled with
/// <see cref="DateOnly"/> because tenancy is reasoned about by day, not time.
/// </summary>
public sealed class AssignmentPeriod : ValueObject
{
    /// <summary>The day the assignment began.</summary>
    public DateOnly StartDate { get; }

    /// <summary>The day the assignment ended, or <c>null</c> while still active.</summary>
    public DateOnly? EndDate { get; }

    /// <summary>True while the assignment has no end date.</summary>
    public bool IsActive => EndDate is null;

    private AssignmentPeriod(DateOnly startDate, DateOnly? endDate)
    {
        StartDate = startDate;
        EndDate = endDate;
    }

    /// <summary>Opens an active, open-ended period beginning on <paramref name="startDate"/>.</summary>
    public static AssignmentPeriod Open(DateOnly startDate) => new(startDate, endDate: null);

    /// <summary>
    /// Returns a closed copy of this period ending on <paramref name="endDate"/>.
    /// </summary>
    /// <exception cref="InvalidAssignmentPeriodException">Thrown when the end date precedes the start date.</exception>
    public AssignmentPeriod CloseOn(DateOnly endDate)
    {
        if (endDate < StartDate)
        {
            throw new InvalidAssignmentPeriodException(StartDate, endDate);
        }

        return new AssignmentPeriod(StartDate, endDate);
    }

    /// <inheritdoc />
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return StartDate;
        yield return EndDate;
    }
}
