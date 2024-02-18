using AutoMapper;
using MediatR;
using SiteManagement.Application.CrossCuttingConcerns.Caching;
using SiteManagement.Application.Services.Repositories.Buildings;
using SiteManagement.Domain.Constants.Buildings.Blocks;
using SiteManagement.Domain.Entities.Buildings;
using SiteManagement.Domain.Exceptions;
using SiteManagement.Domain.Utulity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.Application.Features.Commands.Buildings.Blocks.CreateBlock
{
    public class CreateBlockCommandHandler : IRequestHandler<CreateBlockCommand, Guid>
    {
        private readonly IBlockRepository _blockRepository;
        private readonly IMapper _mapper;

        public CreateBlockCommandHandler(IBlockRepository blockRepository, IMapper mapper)
        {
            _blockRepository = blockRepository;
            _mapper = mapper;
        }

        public async Task<Guid> Handle(CreateBlockCommand request, CancellationToken cancellationToken)
        {
           
            var dbBlock = await _blockRepository.GetSingleAsync(block => block.Name == request.Name);
            if(dbBlock is not null)
                new DatabaseValidationException(BlockMessages.RuleMessages.BlockNameAlreadyExist);

            var blockToAdd = _mapper.Map<Block>(request);
            await _blockRepository.AddAsync(blockToAdd);

            return blockToAdd.Id;

        }
    }
}
