using SiteManagement.Application.Security.JWT;

namespace SiteManagement.Application.Features.Commands.Residents.Login;

public class ResidentLoginCommandResponse
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public AccessToken AccessToken { get; set; }
}