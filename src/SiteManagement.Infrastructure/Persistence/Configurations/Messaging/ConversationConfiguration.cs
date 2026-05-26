using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SiteManagement.Domain.Messaging;

namespace SiteManagement.Infrastructure.Persistence.Configurations.Messaging;

/// <summary>
/// EF mapping for the <see cref="Conversation"/> aggregate root. The messages
/// are an owned-collection child table loaded eagerly with the conversation;
/// each carries its sender, body, and the optional read receipt.
/// </summary>
public sealed class ConversationConfiguration : IEntityTypeConfiguration<Conversation>
{
    public void Configure(EntityTypeBuilder<Conversation> builder)
    {
        builder.ToTable(SchemaConstants.Tables.Conversations);

        builder.HasKey(c => c.Id);

        builder.Property<uint>(SchemaConstants.ConcurrencyTokenColumn)
            .HasColumnName(SchemaConstants.ConcurrencyTokenColumn)
            .IsConcurrencyToken()
            .ValueGeneratedOnAddOrUpdate();

        builder.Property(c => c.ResidentId).IsRequired();

        builder.Property(c => c.Subject)
            .HasMaxLength(MessagingLimits.SubjectMaxLength)
            .IsRequired();

        builder.HasIndex(c => c.ResidentId);

        builder.OwnsMany(c => c.Messages, messages =>
        {
            messages.ToTable(SchemaConstants.Tables.Messages);

            messages.WithOwner().HasForeignKey(SchemaConstants.ForeignKeys.ConversationId);
            messages.HasKey(m => m.Id);

            messages.Property(m => m.SenderUserId).IsRequired();

            messages.Property(m => m.SenderRole)
                .HasConversion<string>()
                .HasMaxLength(MessagingLimits.SenderRoleMaxLength)
                .IsRequired();

            messages.Property(m => m.Body)
                .HasMaxLength(MessagingLimits.BodyMaxLength)
                .IsRequired();

            messages.Property(m => m.SentAtUtc).IsRequired();
            messages.Property(m => m.ReadAtUtc);

            messages.HasIndex(SchemaConstants.ForeignKeys.ConversationId, nameof(Message.SentAtUtc));
        });

        builder.Navigation(c => c.Messages)
            .HasField(SchemaConstants.BackingFields.ConversationMessages)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Ignore(c => c.DomainEvents);
    }
}
