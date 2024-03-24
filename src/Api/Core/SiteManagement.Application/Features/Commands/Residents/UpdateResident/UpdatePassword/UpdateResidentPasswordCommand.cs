using MediatR;

namespace SiteManagement.Application.Features.Commands.Residents.UpdateResident.UpdatePassword
{
    public class UpdateResidentPasswordCommand : IRequest
    {
        public Guid Id { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
