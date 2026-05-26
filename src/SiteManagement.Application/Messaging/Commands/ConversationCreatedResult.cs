namespace SiteManagement.Application.Messaging.Commands;

/// <summary>Identifier of a newly started conversation, returned by the start commands.</summary>
public sealed record ConversationCreatedResult(Guid ConversationId);
