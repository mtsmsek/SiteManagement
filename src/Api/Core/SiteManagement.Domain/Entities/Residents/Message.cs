using SiteManagement.Domain.Entities.Commons;

namespace SiteManagement.Domain.Entities.Residents
{
    public class Message : BaseEntity
    {
        public Guid Sender { get; set; }
        public Guid Receiver { get; set; }
        public string Text { get; set; }
        public bool IsSeen { get; set; }

        public virtual Resident Resident{ get; set; }
    }
}
