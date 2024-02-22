using AutoMapper;
using MediatR;
using SiteManagement.Application.Pagination.Responses;
using SiteManagement.Application.Rules.Buildings.Blocks;
using SiteManagement.Application.Services.Repositories.Buildings;

namespace SiteManagement.Application.Features.Queries.Apartments.GetListAllApartmentsByBlockId
{
    public class GetListAllApartmentsByBlockQueryHandler : IRequestHandler<GetListAllApartmentsByBlockQuery, PagedViewModel<GetListAllApartmentsByBlockResponse>>
    {
        private readonly IApartmentRepository _apartmentRepository;
        private readonly IMapper _mapper;
        private readonly BlockBusinessRules _blockBusinessRules;

        public GetListAllApartmentsByBlockQueryHandler(IApartmentRepository apartmentRepository, IMapper mapper, BlockBusinessRules blockBusinessRules)
        {
            _apartmentRepository = apartmentRepository;
            _mapper = mapper;
            _blockBusinessRules = blockBusinessRules;
        }

        public async Task<PagedViewModel<GetListAllApartmentsByBlockResponse>> Handle(GetListAllApartmentsByBlockQuery request, CancellationToken cancellationToken)
        {
           
            if (request.BlockId == Guid.Empty)
            {
                var block = await _blockBusinessRules.BlockShouldBeExistInDatabase(request.BlockName, "Aradığınız blok bulunamadı.");
                request.BlockId = block.Id;
            }

            var apartmentsInBlock = await _apartmentRepository.GetListAsync(predicate: x => x.BlockId == request.BlockId,
                                             orderBy: apartment => apartment.OrderBy(x => x.ApartmentNumber),
                                             includes: x => x.Block,
                                             cancellationToken: cancellationToken);

            var response = _mapper.Map<PagedViewModel<GetListAllApartmentsByBlockResponse>>(apartmentsInBlock);


            return response;


        }
       
    }
}
