using SiteManagement.Domain.Messaging;

namespace SiteManagement.Application.Abstractions.Persistence;

/// <summary>
/// Command-side repository for the <see cref="Conversation"/> aggregate.
/// Read-side projections live behind <see cref="Messaging.Queries.IMessagingQueries"/>.
/// </summary>
public interface IConversationRepository : IRepository<Conversation>
{
}
