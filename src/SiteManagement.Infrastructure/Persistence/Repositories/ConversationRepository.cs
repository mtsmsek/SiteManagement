using Microsoft.EntityFrameworkCore;
using SiteManagement.Application.Abstractions.Persistence;
using SiteManagement.Domain.Messaging;

namespace SiteManagement.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core-backed <see cref="IConversationRepository"/>. Messages are part of the
/// aggregate, so they load eagerly with the conversation.
/// </summary>
public sealed class ConversationRepository(AppDbContext dbContext) : IConversationRepository
{
    private readonly AppDbContext _dbContext = dbContext;

    /// <inheritdoc />
    public Task<Conversation?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => _dbContext.Conversations
            .Include(c => c.Messages)
            .FirstOrDefaultAsync(c => c.Id == id, ct);

    /// <inheritdoc />
    public async Task AddAsync(Conversation conversation, CancellationToken ct = default)
    {
        await _dbContext.Conversations.AddAsync(conversation, ct);
    }

    /// <inheritdoc />
    public void Remove(Conversation conversation) => _dbContext.Conversations.Remove(conversation);
}
