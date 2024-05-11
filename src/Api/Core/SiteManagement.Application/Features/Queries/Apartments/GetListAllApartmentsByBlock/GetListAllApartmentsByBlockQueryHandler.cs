using AutoMapper;
using MediatR;
using SiteManagement.Application.Extensions;
using SiteManagement.Application.Pagination.Responses;
using SiteManagement.Application.Rules.Buildings.Blocks;
using SiteManagement.Application.Services.Repositories.Buildings;
using SiteManagement.Domain.Entities.Buildings;
using System.Linq.Expressions;

namespace SiteManagement.Application.Features.Queries.Apartments.GetListAllApartmentsByBlock
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
          
            Expression<Func<Apartment, bool>> predicate;

            if (request.BlockId.HasValue)
            {
                await _blockBusinessRules.BlockShouldBeExistInDatabase(request.BlockId.Value);
                predicate = apartment => apartment.BlockId == request.BlockId.Value;

            }

            else
            {
                await _blockBusinessRules.BlockShouldBeExistInDatabase(request.BlockName!);

                predicate = apartment => apartment.Block.Name == request.BlockName;
            }

            var apartmentsInBlock = await _apartmentRepository.GetListAsync(predicate: predicate,
                                             orderBy: apartment => apartment.OrderBy(x => x.ApartmentNumber),
                                             includes: x => x.Block,
                                             cancellationToken: cancellationToken);

            return _mapper.Map<PagedViewModel<GetListAllApartmentsByBlockResponse>>(apartmentsInBlock);


      

        }
       
    }
}
