using Microsoft.IdentityModel.Tokens;

namespace SiteManagement.Application.Security.Encryption;

public static class SigningCredentialsHelper
{
     public static SigningCredentials CreateSigningCredentials(SecurityKey securityKey) 
        => new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512Signature);
}
