using AutoMapper;
using MediatR;
using SiteManagement.Application.Pagination.Requests;
using SiteManagement.Application.Pagination.Responses;
using SiteManagement.Application.Rules.Residents;
using SiteManagement.Application.Services.Repositories.Payments;

namespace SiteManagement.Application.Features.Queries.Payments.GetListResidentPayments;

public class GetListResidentPaymentsQueryHandler : PageRequest, IRequestHandler<GetListResidentPaymentsQuery, PagedViewModel<GetListResidentPaymentsResponse>>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IMapper _mapper;
    private readonly ResidentBusinessRules _residentBusinessRules;

    public GetListResidentPaymentsQueryHandler(IPaymentRepository paymentRepository, IMapper mapper)
    {
        _paymentRepository = paymentRepository;
        _mapper = mapper;
    }

    public async Task<PagedViewModel<GetListResidentPaymentsResponse>> Handle(GetListResidentPaymentsQuery request, CancellationToken cancellationToken)
    {
        //check if user exist
        var dbUser = await _residentBusinessRules.CheckIfResidentExistById(request.UserId, cancellationToken);

        var payments = await _paymentRepository.GetListAsync(predicate: payment => payment.ResidentId == request.UserId, cancellationToken: cancellationToken,
                                              orderBy: payment => payment.OrderBy(x => x.CreatedDate),
                                              includes: [payment => payment.Bill, payment => payment.Resident]);

        return _mapper.Map<PagedViewModel<GetListResidentPaymentsResponse>>(payments);


    }
}
