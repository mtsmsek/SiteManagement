using FluentAssertions;
using NSubstitute;
using SiteManagement.Application.Abstractions.Persistence;
using SiteManagement.Application.Billing.Services;
using SiteManagement.Domain.Billing;
using SiteManagement.Domain.Shared.ValueObjects;

namespace SiteManagement.Application.Tests.Billing;

/// <summary>
/// Unit tests for <see cref="ResidentCreditService"/>: open-on-first-credit,
/// top up an existing account, and consume credit only when it fully covers a bill.
/// </summary>
public class ResidentCreditServiceTests
{
    private readonly IResidentCreditAccountRepository _accounts =
        Substitute.For<IResidentCreditAccountRepository>();

    [Fact]
    public async Task ApplyCredits_NoExistingAccount_OpensOneAndStagesIt()
    {
        // arrange — resident has no account yet
        var residentId = Guid.NewGuid();
        _accounts.GetByResidentIdAsync(residentId, Arg.Any<CancellationToken>())
            .Returns((ResidentCreditAccount?)null);
        var sut = CreateService();

        // act
        await sut.ApplyCreditsAsync([new OverpaymentCredit(residentId, Money.Of(400m))], CancellationToken.None);

        // assert — a new account carrying the credit was added
        await _accounts.Received(1).AddAsync(
            Arg.Is<ResidentCreditAccount>(a => a.ResidentId == residentId && a.Balance == Money.Of(400m)),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ApplyCredits_ExistingAccount_TopsItUp_WithoutAdding()
    {
        // arrange — resident already holds 100 credit
        var residentId = Guid.NewGuid();
        var account = ResidentCreditAccount.Open(residentId);
        account.AddCredit(Money.Of(100m));
        _accounts.GetByResidentIdAsync(residentId, Arg.Any<CancellationToken>()).Returns(account);
        var sut = CreateService();

        // act
        await sut.ApplyCreditsAsync([new OverpaymentCredit(residentId, Money.Of(400m))], CancellationToken.None);

        // assert — balance grows, no new account staged
        account.Balance.Should().Be(Money.Of(500m));
        await _accounts.DidNotReceive().AddAsync(Arg.Any<ResidentCreditAccount>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task TryConsume_WhenCreditCovers_ReturnsTrue_AndReducesBalance()
    {
        // arrange — 1,500 credit against a 1,500 bill
        var residentId = Guid.NewGuid();
        var account = ResidentCreditAccount.Open(residentId);
        account.AddCredit(Money.Of(1_500m));
        _accounts.GetByResidentIdAsync(residentId, Arg.Any<CancellationToken>()).Returns(account);
        var sut = CreateService();

        // act
        var consumed = await sut.TryConsumeAsync(residentId, Money.Of(1_500m), CancellationToken.None);

        // assert
        consumed.Should().BeTrue();
        account.Balance.Should().Be(Money.Zero);
    }

    [Fact]
    public async Task TryConsume_NoAccount_ReturnsFalse()
    {
        // arrange
        var residentId = Guid.NewGuid();
        _accounts.GetByResidentIdAsync(residentId, Arg.Any<CancellationToken>())
            .Returns((ResidentCreditAccount?)null);
        var sut = CreateService();

        // act
        var consumed = await sut.TryConsumeAsync(residentId, Money.Of(100m), CancellationToken.None);

        // assert
        consumed.Should().BeFalse();
    }

    private ResidentCreditService CreateService() => new(_accounts);
}
