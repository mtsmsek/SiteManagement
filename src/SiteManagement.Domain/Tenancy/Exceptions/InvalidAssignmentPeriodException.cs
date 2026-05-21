using SiteManagement.Domain.Shared.Exceptions;

namespace SiteManagement.Domain.Tenancy.Exceptions;

/// <summary>
/// Thrown when an assignment's end date precedes its start date.
/// </summary>
public sealed class InvalidAssignmentPeriodException : DomainException
{
    /// <summary>Creates the exception for the offending date range.</summary>
    public InvalidAssignmentPeriodException(DateOnly start, DateOnly end)
        : base(TenancyMessageKeys.AssignmentPeriodInvalid, start, end)
    {
    }
}
