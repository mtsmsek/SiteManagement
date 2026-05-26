using Microsoft.EntityFrameworkCore;
using SiteManagement.Application.Messaging.Queries;
using SiteManagement.Domain.Messaging;

namespace SiteManagement.Infrastructure.Persistence.Queries;

/// <summary>
/// EF Core-backed <see cref="IMessagingQueries"/>. Projects conversations and
/// their messages straight into DTOs, computing per-side unread counts in the
/// query. No domain materialisation, no change tracking.
/// </summary>
public sealed class MessagingQueries(AppDbContext dbContext) : IMessagingQueries
{
    private readonly AppDbContext _dbContext = dbContext;

    /// <inheritdoc />
    public Task<IReadOnlyList<ConversationListItemDto>> ListAllAsync(CancellationToken ct = default)
        => ProjectListAsync(_dbContext.Conversations.AsNoTracking(), ct);

    /// <inheritdoc />
    public Task<IReadOnlyList<ConversationListItemDto>> ListForResidentAsync(Guid residentId, CancellationToken ct = default)
        => ProjectListAsync(_dbContext.Conversations.AsNoTracking().Where(c => c.ResidentId == residentId), ct);

    /// <inheritdoc />
    public async Task<IReadOnlyList<MessageDto>> ListMessagesAsync(Guid conversationId, CancellationToken ct = default)
        => await _dbContext.Conversations
            .AsNoTracking()
            .Where(c => c.Id == conversationId)
            .SelectMany(c => c.Messages)
            .OrderBy(m => m.SentAtUtc)
            .Select(m => new MessageDto(m.Id, m.SenderRole.ToString(), m.Body, m.SentAtUtc, m.ReadAtUtc))
            .ToListAsync(ct);

    /// <inheritdoc />
    public async Task<Guid?> FindResidentIdAsync(Guid conversationId, CancellationToken ct = default)
        => await _dbContext.Conversations
            .AsNoTracking()
            .Where(c => c.Id == conversationId)
            .Select(c => (Guid?)c.ResidentId)
            .FirstOrDefaultAsync(ct);

    private static async Task<IReadOnlyList<ConversationListItemDto>> ProjectListAsync(
        IQueryable<Conversation> query, CancellationToken ct)
        => await query
            .OrderByDescending(c => c.Messages.Max(m => m.SentAtUtc))
            .Select(c => new ConversationListItemDto(
                c.Id,
                c.ResidentId,
                c.Subject,
                c.Messages.Count,
                c.Messages.Max(m => m.SentAtUtc),
                c.Messages.Count(m => m.SenderRole == MessageSenderRole.Resident && m.ReadAtUtc == null),
                c.Messages.Count(m => m.SenderRole == MessageSenderRole.Admin && m.ReadAtUtc == null)))
            .ToListAsync(ct);
}
