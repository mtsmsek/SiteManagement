using FluentAssertions;
using NSubstitute;
using SiteManagement.Application.Abstractions.Persistence;
using SiteManagement.Application.Shared.Exceptions;
using SiteManagement.Application.Tenancy.Commands.EndAssignment;
using SiteManagement.Domain.Tenancy;

namespace SiteManagement.Application.Tests.Tenancy;

/// <summary>
/// Unit tests for <see cref="EndAssignmentCommandHandler"/>: load the assignment,
/// end it, and save — or 404 when the assignment is unknown.
/// </summary>
public class EndAssignmentCommandHandlerTests
{
    private readonly IApartmentAssignmentRepository _assignmentRepository = Substitute.For<IApartmentAssignmentRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    [Fact]
    public async Task Handle_EndsTheAssignmentAndSaves()
    {
        // arrange — an active assignment
        var assignment = ApartmentAssignment.Assign(
            Guid.NewGuid(), Guid.NewGuid(), TenantType.Owner, new DateOnly(2026, 1, 1));
        _assignmentRepository.GetByIdAsync(assignment.Id, Arg.Any<CancellationToken>()).Returns(assignment);
        var sut = CreateHandler();

        // act
        await sut.Handle(new EndAssignmentCommand(assignment.Id, new DateOnly(2026, 2, 1)), CancellationToken.None);

        // assert
        assignment.IsActive.Should().BeFalse();
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_UnknownAssignment_Throws()
    {
        // arrange
        _assignmentRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((ApartmentAssignment?)null);
        var sut = CreateHandler();

        // act
        var act = () => sut.Handle(new EndAssignmentCommand(Guid.NewGuid(), new DateOnly(2026, 2, 1)), CancellationToken.None);

        // assert
        await act.Should().ThrowAsync<EntityNotFoundException>();
    }

    private EndAssignmentCommandHandler CreateHandler() => new(_assignmentRepository, _unitOfWork);
}
