using FluentValidation;
using SiteManagement.Application.Shared.Validation;

namespace SiteManagement.Application.Messaging.Commands.StartMyConversation;

/// <summary>Surface-level validation for <see cref="StartMyConversationCommand"/>.</summary>
public sealed class StartMyConversationCommandValidator : AbstractValidator<StartMyConversationCommand>
{
    public StartMyConversationCommandValidator()
    {
        RuleFor(x => x.Subject).RequiredText();
        RuleFor(x => x.Body).RequiredText();
    }
}
