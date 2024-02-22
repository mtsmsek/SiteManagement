using AutoMapper;
using MediatR;
using SiteManagement.Application.Rules.Buildings.Blocks;
using SiteManagement.Application.Services.Repositories.Buildings;
using SiteManagement.Domain.Constants.Buildings.Blocks;
using SiteManagement.Domain.Entities.Buildings;

namespace SiteManagement.Application.Features.Commands.Buildings.Blocks.CreateBlock
{
    public class CreateBlockCommandHandler : IRequestHandler<CreateBlockCommand, Guid>
    {
        private readonly IBlockRepository _blockRepository;
        private readonly IMapper _mapper;
        private readonly BlockBusinessRules _blockBusinessRules;

        public CreateBlockCommandHandler(IBlockRepository blockRepository, IMapper mapper, BlockBusinessRules brandBusinessRules)
        {
            _blockRepository = blockRepository;
            _mapper = mapper;
            _blockBusinessRules = brandBusinessRules;
        }

        public async Task<Guid> Handle(CreateBlockCommand request, CancellationToken cancellationToken)
        {

            await _blockBusinessRules.BlockNameCannotBeDublicateWhenAddOrUpdate(request.Name, BlockMessages.RuleMessages.BlockNameAlreadyExist);
           
            var blockToAdd = _mapper.Map<Block>(request);
            await _blockRepository.AddAsync(blockToAdd);

            return blockToAdd.Id;

        }
       
    }
}
