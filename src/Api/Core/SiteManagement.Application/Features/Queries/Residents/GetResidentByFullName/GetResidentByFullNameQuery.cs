using MediatR;

namespace SiteManagement.Application.Features.Queries.Residents.GetResidentByFullName;

public class GetResidentByFullNameQuery : IRequest<GetResidentByFullNameResponse>
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
}
