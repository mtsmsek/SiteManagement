using FluentValidation;
using SiteManagement.Application.Shared.Validation;

namespace SiteManagement.Application.Messaging.Commands.StartConversation;

/// <summary>
/// Surface-level validation for <see cref="StartConversationCommand"/>. Subject
/// and body length caps are domain invariants; here we only check presence.
/// </summary>
public sealed class StartConversationCommandValidator : AbstractValidator<StartConversationCommand>
{
    public StartConversationCommandValidator()
    {
        RuleFor(x => x.ResidentId).ValidId();
        RuleFor(x => x.Subject).RequiredText();
        RuleFor(x => x.Body).RequiredText();
    }
}
