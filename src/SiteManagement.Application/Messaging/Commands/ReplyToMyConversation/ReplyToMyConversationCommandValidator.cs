using FluentValidation;
using SiteManagement.Application.Shared.Validation;

namespace SiteManagement.Application.Messaging.Commands.ReplyToMyConversation;

/// <summary>Surface-level validation for <see cref="ReplyToMyConversationCommand"/>.</summary>
public sealed class ReplyToMyConversationCommandValidator : AbstractValidator<ReplyToMyConversationCommand>
{
    public ReplyToMyConversationCommandValidator()
    {
        RuleFor(x => x.ConversationId).ValidId();
        RuleFor(x => x.Body).RequiredText();
    }
}
