using MediatR;
using SiteManagement.Application.Pagination.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.Application.Features.Queries.Payments.GetListResidentPayments
{
    public class GetListResidentPaymentsQuery : IRequest<PagedViewModel<GetListResidentPaymentsResponse>>
    {
        public Guid UserId { get; set; }

    }
}
