using MediatR;

namespace SiteManagement.Application.Features.Commands.Residents.Login;

public class ResidentLoginCommand : IRequest<ResidentLoginCommandResponse>
{
    public string? Email { get; set; }
    public string? IdenticalNumber { get; set; }
    public string Password { get; set; }

}
