using MediatR;

namespace SiteManagement.Application.Features.Queries.Residents.GetResidentByIdenticalNumber
{
    public class GetResidentByIdenticalNumberQuery : IRequest<GetResidentByIdenticalNumberResponse>
    {
        public string IdenticalNumber { get; set; }
    }
}
