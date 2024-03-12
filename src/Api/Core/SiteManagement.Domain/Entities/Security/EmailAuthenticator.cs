using SiteManagement.Domain.Entities.Commons;

namespace SiteManagement.Domain.Entities.Security
{
    public class EmailAuthenticator : BaseEntity
    {
        public Guid UserId { get; set; }
        public string? ActivationKey { get; set; }
        public bool IsVerified { get; set; }
        public virtual User User { get; set; }

    }
}
