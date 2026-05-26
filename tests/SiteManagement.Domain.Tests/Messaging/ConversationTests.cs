using FluentAssertions;
using SiteManagement.Domain.Messaging;
using SiteManagement.Domain.Messaging.Exceptions;

namespace SiteManagement.Domain.Tests.Messaging;

/// <summary>
/// Invariants for the <see cref="Conversation"/> aggregate: it always opens with
/// one valid message, appends replies, and read receipts only ever touch the
/// other side's messages.
/// </summary>
public class ConversationTests
{
    private static readonly Guid Resident = Guid.NewGuid();
    private static readonly Guid AdminUser = Guid.NewGuid();
    private static readonly Guid ResidentUser = Guid.NewGuid();
    private static readonly DateTime Now = new(2026, 5, 27, 9, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void Start_CreatesConversationWithOpeningMessage()
    {
        // act
        var conversation = Conversation.Start(Resident, "Su kaçağı", MessageSenderRole.Resident, ResidentUser, "Merhaba", Now);

        // assert
        conversation.ResidentId.Should().Be(Resident);
        conversation.Subject.Should().Be("Su kaçağı");
        conversation.Messages.Should().ContainSingle();
        var opener = conversation.Messages.Single();
        opener.SenderRole.Should().Be(MessageSenderRole.Resident);
        opener.SenderUserId.Should().Be(ResidentUser);
        opener.Body.Should().Be("Merhaba");
        opener.SentAtUtc.Should().Be(Now);
        opener.ReadAtUtc.Should().BeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Start_WithBlankSubject_Throws(string subject)
    {
        // act
        var act = () => Conversation.Start(Resident, subject, MessageSenderRole.Admin, AdminUser, "body", Now);

        // assert
        act.Should().Throw<InvalidConversationSubjectException>();
    }

    [Fact]
    public void Start_WithTooLongSubject_Throws()
    {
        // arrange
        var subject = new string('x', MessagingLimits.SubjectMaxLength + 1);

        // act
        var act = () => Conversation.Start(Resident, subject, MessageSenderRole.Admin, AdminUser, "body", Now);

        // assert
        act.Should().Throw<InvalidConversationSubjectException>();
    }

    [Fact]
    public void Start_WithBlankBody_Throws()
    {
        // act
        var act = () => Conversation.Start(Resident, "Subject", MessageSenderRole.Admin, AdminUser, "  ", Now);

        // assert
        act.Should().Throw<InvalidMessageBodyException>();
    }

    [Fact]
    public void PostMessage_AppendsReplyAndReturnsIt()
    {
        // arrange
        var conversation = Conversation.Start(Resident, "Subject", MessageSenderRole.Resident, ResidentUser, "Soru", Now);

        // act
        var reply = conversation.PostMessage(AdminUser, MessageSenderRole.Admin, "Cevap", Now.AddMinutes(5));

        // assert
        conversation.Messages.Should().HaveCount(2);
        conversation.Messages.Last().Should().BeSameAs(reply);
        reply.SenderRole.Should().Be(MessageSenderRole.Admin);
        reply.Body.Should().Be("Cevap");
    }

    [Fact]
    public void PostMessage_WithTooLongBody_Throws()
    {
        // arrange
        var conversation = Conversation.Start(Resident, "Subject", MessageSenderRole.Resident, ResidentUser, "Soru", Now);
        var body = new string('x', MessagingLimits.BodyMaxLength + 1);

        // act
        var act = () => conversation.PostMessage(AdminUser, MessageSenderRole.Admin, body, Now);

        // assert
        act.Should().Throw<InvalidMessageBodyException>();
    }

    [Fact]
    public void MarkRead_MarksTheOtherSidesMessages_NotTheReadersOwn()
    {
        // arrange — admin opens, resident replies
        var conversation = Conversation.Start(Resident, "Subject", MessageSenderRole.Admin, AdminUser, "Duyuru", Now);
        conversation.PostMessage(ResidentUser, MessageSenderRole.Resident, "Teşekkürler", Now.AddMinutes(1));

        // act — the resident reads the thread
        conversation.MarkRead(MessageSenderRole.Resident, Now.AddMinutes(2));

        // assert — admin's message is read, the resident's own stays unread
        var adminMessage = conversation.Messages.Single(m => m.SenderRole == MessageSenderRole.Admin);
        var residentMessage = conversation.Messages.Single(m => m.SenderRole == MessageSenderRole.Resident);
        adminMessage.ReadAtUtc.Should().Be(Now.AddMinutes(2));
        residentMessage.ReadAtUtc.Should().BeNull();
    }

    [Fact]
    public void MarkRead_IsIdempotent_KeepingTheFirstTimestamp()
    {
        // arrange
        var conversation = Conversation.Start(Resident, "Subject", MessageSenderRole.Admin, AdminUser, "Duyuru", Now);
        var firstRead = Now.AddMinutes(2);
        conversation.MarkRead(MessageSenderRole.Resident, firstRead);

        // act — a second read later doesn't move the timestamp
        conversation.MarkRead(MessageSenderRole.Resident, Now.AddMinutes(10));

        // assert
        conversation.Messages.Single().ReadAtUtc.Should().Be(firstRead);
    }
}
