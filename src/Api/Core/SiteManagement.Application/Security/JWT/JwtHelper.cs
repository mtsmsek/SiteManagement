using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SiteManagement.Application.Security.Encryption;
using SiteManagement.Application.Security.Extensions;
using SiteManagement.Domain.Entities.Security;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.Application.Security.JWT;

public class JwtHelper : ITokenHelper
{
    private readonly IConfiguration _configuration;
    private readonly TokenOptions _tokenOptions;
    private DateTime _accessTokenExpiration;
    //TODO Add refresh token
    public JwtHelper(IConfiguration configuration, TokenOptions tokenOptions, DateTime accessTokenExpiration)
    {
        _configuration = configuration;
        const string configurationSection = "TokenOptions";
        _tokenOptions = _configuration.GetSection(configurationSection)
                                        .Get<TokenOptions>()
                                        ?? throw new NullReferenceException($"\"{configurationSection}\" section cannot found in configuration");

        _accessTokenExpiration = accessTokenExpiration;
    }

    public AccessToken CreateToken(User user, IList<OperationClaim> operationClaims)
    {
        _accessTokenExpiration = DateTime.Now.AddMinutes(_tokenOptions.AccessTokenExpiration);
        SecurityKey securityKey = SecurityKeyHelper.CreateSecurityKey(_tokenOptions.SecurityKey);
        SigningCredentials signingCredentials = SigningCredentialsHelper.CreateSigningCredentials(securityKey);
        JwtSecurityToken jwt = null;


        return new AccessToken();
    }

    private JwtSecurityToken createJwtSecurityToken(TokenOptions tokenOptions,
                                                    User user,
                                                    SigningCredentials signingCredentials,
                                                    IList<OperationClaim> operationClaims)
    {
        JwtSecurityToken jwt = new(issuer: tokenOptions.Issuer,
                                    audience: tokenOptions.Auidience,
                                    expires: _accessTokenExpiration,
                                    notBefore: DateTime.Now,
                                    claims: SetClaims(user,operationClaims),
                                    signingCredentials: signingCredentials);

        return jwt;

    }

    private IEnumerable<Claim> SetClaims(User user, IList<OperationClaim> operationClaims)
    {
        List<Claim> claims = new();
        claims.AddNameIdentifier(user.Id.ToString());
        claims.AddEmail(user.Email);
        claims.AddName($"{user.FirstName} {user.LastName}");
        claims.AddRoles(operationClaims.Select(c => c.Name).ToArray());

        return claims;
    }

}
