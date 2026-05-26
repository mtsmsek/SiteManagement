using FluentValidation;
using SiteManagement.Application.Shared.Validation;

namespace SiteManagement.Application.Messaging.Commands.MarkConversationRead;

/// <summary>Surface-level validation for <see cref="MarkConversationReadCommand"/>.</summary>
public sealed class MarkConversationReadCommandValidator : AbstractValidator<MarkConversationReadCommand>
{
    public MarkConversationReadCommandValidator()
    {
        RuleFor(x => x.ConversationId).ValidId();
    }
}
