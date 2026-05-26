using FluentValidation;
using SiteManagement.Application.Shared.Validation;

namespace SiteManagement.Application.Messaging.Commands.MarkMyConversationRead;

/// <summary>Surface-level validation for <see cref="MarkMyConversationReadCommand"/>.</summary>
public sealed class MarkMyConversationReadCommandValidator : AbstractValidator<MarkMyConversationReadCommand>
{
    public MarkMyConversationReadCommandValidator()
    {
        RuleFor(x => x.ConversationId).ValidId();
    }
}
