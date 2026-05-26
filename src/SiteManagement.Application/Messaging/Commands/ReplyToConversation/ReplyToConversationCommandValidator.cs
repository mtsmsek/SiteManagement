using FluentValidation;
using SiteManagement.Application.Shared.Validation;

namespace SiteManagement.Application.Messaging.Commands.ReplyToConversation;

/// <summary>Surface-level validation for <see cref="ReplyToConversationCommand"/>.</summary>
public sealed class ReplyToConversationCommandValidator : AbstractValidator<ReplyToConversationCommand>
{
    public ReplyToConversationCommandValidator()
    {
        RuleFor(x => x.ConversationId).ValidId();
        RuleFor(x => x.Body).RequiredText();
    }
}
