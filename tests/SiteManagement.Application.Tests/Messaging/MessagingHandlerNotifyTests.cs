using FluentAssertions;
using NSubstitute;
using SiteManagement.Application.Abstractions.Auth;
using SiteManagement.Application.Abstractions.Persistence;
using SiteManagement.Application.Messaging.Commands.MarkConversationRead;
using SiteManagement.Application.Messaging.Commands.MarkMyConversationRead;
using SiteManagement.Application.Messaging.Commands.ReplyToConversation;
using SiteManagement.Application.Messaging.Commands.ReplyToMyConversation;
using SiteManagement.Application.Messaging.Commands.StartConversation;
using SiteManagement.Application.Messaging.Commands.StartMyConversation;
using SiteManagement.Application.Messaging.Notifications;
using SiteManagement.Domain.Messaging;

namespace SiteManagement.Application.Tests.Messaging;

/// <summary>
/// Smoke tests proving every messaging command handler pushes the right
/// real-time notification through <see cref="IMessagingNotifier"/>: admin-side
/// handlers notify the affected resident, resident-side handlers notify every
/// admin. Full happy-path persistence is covered by the messaging E2E.
/// </summary>
public class MessagingHandlerNotifyTests
{
    private static readonly DateTime Now = new(2026, 5, 1, 9, 0, 0, DateTimeKind.Utc);
    private static readonly Guid AdminUserId = Guid.NewGuid();
    private static readonly Guid ResidentUserId = Guid.NewGuid();
    private static readonly Guid ResidentId = Guid.NewGuid();

    private readonly IConversationRepository _repo = Substitute.For<IConversationRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly ICurrentUser _currentUser = Substitute.For<ICurrentUser>();
    private readonly TimeProvider _time = Substitute.For<TimeProvider>();
    private readonly IMessagingNotifier _notifier = Substitute.For<IMessagingNotifier>();

    public MessagingHandlerNotifyTests()
    {
        _time.GetUtcNow().Returns(new DateTimeOffset(Now));
        _currentUser.UserId.Returns(AdminUserId);
        _currentUser.ResidentId.Returns(ResidentId);
    }

    [Fact]
    public async Task StartConversation_NotifiesResident_WithConversationStarted()
    {
        // arrange
        var sut = new StartConversationCommandHandler(_repo, _currentUser, _time, _unitOfWork, _notifier);

        // act
        await sut.Handle(new StartConversationCommand(ResidentId, "Aidat", "Merhaba"), CancellationToken.None);

        // assert
        await _notifier.Received(1).NotifyResidentAsync(
            ResidentId,
            Arg.Is<ConversationStartedNotification>(n => n.ResidentId == ResidentId),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ReplyToConversation_NotifiesResident_WithMessageReceived()
    {
        // arrange
        var conversation = Conversation.Start(ResidentId, "S", MessageSenderRole.Admin, AdminUserId, "B", Now);
        _repo.GetByIdAsync(conversation.Id, Arg.Any<CancellationToken>()).Returns(conversation);
        var sut = new ReplyToConversationCommandHandler(_repo, _currentUser, _time, _unitOfWork, _notifier);

        // act
        await sut.Handle(new ReplyToConversationCommand(conversation.Id, "Yanıt"), CancellationToken.None);

        // assert
        await _notifier.Received(1).NotifyResidentAsync(
            ResidentId,
            Arg.Is<MessageReceivedNotification>(n => n.ConversationId == conversation.Id),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task MarkConversationRead_NotifiesResident_WithMessageRead()
    {
        // arrange
        var conversation = Conversation.Start(ResidentId, "S", MessageSenderRole.Resident, ResidentUserId, "B", Now);
        _repo.GetByIdAsync(conversation.Id, Arg.Any<CancellationToken>()).Returns(conversation);
        var sut = new MarkConversationReadCommandHandler(_repo, _time, _unitOfWork, _notifier);

        // act
        await sut.Handle(new MarkConversationReadCommand(conversation.Id), CancellationToken.None);

        // assert
        await _notifier.Received(1).NotifyResidentAsync(
            ResidentId,
            Arg.Is<MessageReadNotification>(n => n.ConversationId == conversation.Id),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task StartMyConversation_NotifiesAdmins_WithConversationStarted()
    {
        // arrange
        _currentUser.UserId.Returns(ResidentUserId);
        var sut = new StartMyConversationCommandHandler(_repo, _currentUser, _time, _unitOfWork, _notifier);

        // act
        await sut.Handle(new StartMyConversationCommand("Soru", "İçerik"), CancellationToken.None);

        // assert
        await _notifier.Received(1).NotifyAdminsAsync(
            Arg.Is<ConversationStartedNotification>(n => n.ResidentId == ResidentId),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ReplyToMyConversation_NotifiesAdmins_WithMessageReceived()
    {
        // arrange
        var conversation = Conversation.Start(ResidentId, "S", MessageSenderRole.Admin, AdminUserId, "B", Now);
        _repo.GetByIdAsync(conversation.Id, Arg.Any<CancellationToken>()).Returns(conversation);
        _currentUser.UserId.Returns(ResidentUserId);
        var sut = new ReplyToMyConversationCommandHandler(_repo, _currentUser, _time, _unitOfWork, _notifier);

        // act
        await sut.Handle(new ReplyToMyConversationCommand(conversation.Id, "Yanıt"), CancellationToken.None);

        // assert
        await _notifier.Received(1).NotifyAdminsAsync(
            Arg.Is<MessageReceivedNotification>(n => n.ConversationId == conversation.Id),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task MarkMyConversationRead_NotifiesAdmins_WithMessageRead()
    {
        // arrange
        var conversation = Conversation.Start(ResidentId, "S", MessageSenderRole.Admin, AdminUserId, "B", Now);
        _repo.GetByIdAsync(conversation.Id, Arg.Any<CancellationToken>()).Returns(conversation);
        var sut = new MarkMyConversationReadCommandHandler(_repo, _time, _unitOfWork, _notifier);

        // act
        await sut.Handle(new MarkMyConversationReadCommand(conversation.Id), CancellationToken.None);

        // assert
        await _notifier.Received(1).NotifyAdminsAsync(
            Arg.Is<MessageReadNotification>(n => n.ConversationId == conversation.Id),
            Arg.Any<CancellationToken>());
    }
}
