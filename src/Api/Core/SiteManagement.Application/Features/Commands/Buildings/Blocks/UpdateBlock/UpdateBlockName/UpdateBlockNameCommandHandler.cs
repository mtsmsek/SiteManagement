using AutoMapper;
using MediatR;
using SiteManagement.Application.Rules.Buildings.Blocks;
using SiteManagement.Application.Services.Repositories.Buildings;
using SiteManagement.Domain.Constants.Buildings.Blocks;
using SiteManagement.Domain.Entities.Buildings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.Application.Features.Commands.Buildings.Blocks.UpdateBlock.UpdateBlockName
{
    public class UpdateBlockNameCommandHandler : IRequestHandler<UpdateBlockNameCommand, UpdateBlockNameResponse>
    {
        private readonly IBlockRepository _blockRepository;
        private readonly IMapper _mapper;
        private readonly BlockBusinessRules _blockBusinessRules;

        public UpdateBlockNameCommandHandler(IBlockRepository blockRepository, IMapper mapper, BlockBusinessRules blockBusinessRules) : this(blockRepository, mapper)
        {
            _blockBusinessRules = blockBusinessRules;
        }

        public UpdateBlockNameCommandHandler(IBlockRepository blockRepository, IMapper mapper)
        {
            _blockRepository = blockRepository;
            _mapper = mapper;
        }

        public async Task<UpdateBlockNameResponse> Handle(UpdateBlockNameCommand request, CancellationToken cancellationToken)
        {
            //TODO -- existleri singleAsync ile çöz
            Block block = await _blockBusinessRules.BlockShouldBeExistInDatabase(request.Id,BlockMessages.RuleMessages.BlocIsNotExist);
            
            await _blockBusinessRules.BlockNameCannotBeDublicateWhenAddOrUpdate(request.Name, BlockMessages.RuleMessages.BlockNameAlreadyExist);
            
            var oldName = block.Name;

            block = _mapper.Map(request, block);

            await _blockRepository.UpdateAsync(block);
            
           
            return new UpdateBlockNameResponse (oldName,request.Name, block.UpdatedDate!.Value);
         
        }
    }
}
