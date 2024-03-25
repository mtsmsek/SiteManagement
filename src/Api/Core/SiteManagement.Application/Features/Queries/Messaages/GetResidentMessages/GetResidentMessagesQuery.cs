using MediatR;
using SiteManagement.Application.Pagination.Responses;

namespace SiteManagement.Application.Features.Queries.Messaages.GetResidentMessages;

public class GetResidentMessagesQuery : IRequest<PagedViewModel<GetResidentMessagesResponse>>
{
    public Guid UserId { get; set; }

}
