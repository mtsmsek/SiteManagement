using AutoMapper;
using MediatR;
using SiteManagement.Application.CrossCuttingConcerns.Exceptions.Types;
using SiteManagement.Application.Services.Repositories.Invoices;
using SiteManagement.Application.Services.Repositories.Payments;
using SiteManagement.Application.Services.Repositories.Residents;

namespace SiteManagement.Application.Features.Commands.Payments.UpdatePayment;

public class UpdatePaymentCommandHandler : IRequestHandler<UpdatePaymentCommand, UpdatePaymentResponse>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IMapper _mapper;
    private readonly IResidentRepository _residentRepository;
    private readonly IBillReposiotry _billRepository;

    public UpdatePaymentCommandHandler(IPaymentRepository paymentRepository, IMapper mapper, IBillReposiotry billRepository, IResidentRepository residentRepository)
    {
        _paymentRepository = paymentRepository;
        _mapper = mapper;
        _billRepository = billRepository;
        _residentRepository = residentRepository;
    }

    public async Task<UpdatePaymentResponse> Handle(UpdatePaymentCommand request, CancellationToken cancellationToken)
    {

        var dbBill = await _billRepository.GetByIdAsync(request.BillId);
        if (dbBill is null)
            throw new BusinessException("İlgili fatura bulunamadı");

        var dbPayment = await _paymentRepository.GetByIdAsync(request.Id);
        //todo -- remove magic string
        if (dbPayment is null)
            throw new BusinessException("İlgili ödeme bulunamadı");

        _mapper.Map(request, dbPayment);

        await _paymentRepository.UpdateAsync(dbPayment, cancellationToken);

        return _mapper.Map<UpdatePaymentResponse>(dbPayment);

    }
}
