using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SiteManagement.Domain.Entities.Residents;
using SiteManagement.Persistance.Configurations.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.Persistance.Configurations.Residents
{
    public class MessageConfiguration : BaseEntityConfiguration<Message>
    {
        public override void Configure(EntityTypeBuilder<Message> builder)
        {

            builder.Property(message => message.SenderId).IsRequired();
            builder.Property(message => message.ReceiverId).IsRequired();
            builder.Property(message => message.Text).IsRequired();
            builder.Property(message => message.IsSeen).IsRequired();

            builder.HasOne(message => message.Sender)
                    .WithMany(message => message.SentMessages)
                    .HasForeignKey(message => message.SenderId)
                    .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(message => message.Receiver)
                    .WithMany(user => user.ReceivedMessages)
                    .HasForeignKey(message => message.ReceiverId)
                    .OnDelete(DeleteBehavior.Restrict);






        }
    }
}
