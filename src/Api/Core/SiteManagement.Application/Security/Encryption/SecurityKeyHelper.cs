using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace SiteManagement.Application.Security.Encryption;

public class SecurityKeyHelper
{
    public static SecurityKey CreateSecurityKey(string securityKey)
        => new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey));
}
