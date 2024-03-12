using SiteManagement.Domain.Entities.Commons;
using SiteManagement.Domain.Enumarations.Security;

namespace SiteManagement.Domain.Entities.Security;

public class User : BaseEntity
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public byte[] PasswordSalt { get; set; }
    public byte[] PasswordHash { get; set;}
    public AuthenticatorType AuthenticatorType { get; set; }

    public virtual ICollection<UserOperationClaim> UserOperationClaims{ get; set; }
    public virtual ICollection<EmailAuthenticator> EmailAuthenticators{ get; set; }


}
