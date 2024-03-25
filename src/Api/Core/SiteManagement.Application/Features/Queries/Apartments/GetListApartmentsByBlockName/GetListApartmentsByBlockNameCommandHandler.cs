using AutoMapper;
using MediatR;
using SiteManagement.Application.Pagination.Responses;
using SiteManagement.Application.Rules.Buildings.Blocks;
using SiteManagement.Application.Services.Repositories.Buildings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.Application.Features.Queries.Apartments.GetListApartmentsByBlockName
{
    public class GetListApartmentsByBlockNameCommandHandler : IRequestHandler<GetListApartmentsByBlockNameCommand,
                                                                                 PagedViewModel<GetListApartmentsByBlockNameResponse>>
    {
        private readonly IApartmentRepository _apartmentRepository;
        private readonly IMapper _mapper;
        private readonly BlockBusinessRules _blockBusinessRules;

        public GetListApartmentsByBlockNameCommandHandler(IApartmentRepository apartmentRepository, IMapper mapper, BlockBusinessRules blockBusinessRules)
        {
            _apartmentRepository = apartmentRepository;
            _mapper = mapper;
            _blockBusinessRules = blockBusinessRules;
        }

        public async Task<PagedViewModel<GetListApartmentsByBlockNameResponse>> Handle(GetListApartmentsByBlockNameCommand request, CancellationToken cancellationToken)
        {
            //TODO remove the message
            var block = await _blockBusinessRules.BlockShouldBeExistInDatabase(request.BlockName, "Block cannot find");

            var apartmentList = await _apartmentRepository.GetListAsync(predicate: apartment => apartment.BlockId == block.Id,
                                                    includes: include => include.Block);

            var response = _mapper.Map<PagedViewModel<GetListApartmentsByBlockNameResponse>>(apartmentList);

            return response;

        }
    }
}
