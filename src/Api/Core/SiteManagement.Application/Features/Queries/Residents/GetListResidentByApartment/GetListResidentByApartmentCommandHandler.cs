using MediatR;
using SiteManagement.Application.Pagination.Responses;

namespace SiteManagement.Application.Features.Queries.Residents.GetListResidentByApartment;

public class GetListResidentByApartmentCommandHandler : IRequestHandler<GetListResidentByApartmentCommand,
                                                                          PagedViewModel<GetListResidentByApartmentResponse>>
{
    public Task<PagedViewModel<GetListResidentByApartmentResponse>> Handle(GetListResidentByApartmentCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
