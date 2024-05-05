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

namespace SiteManagement.Application.Features.Commands.Buildings.Blocks.DeleteBlock.HardDelete;

public class HardDeleteBlockCommandHandler : IRequestHandler<HardDeleteBlockCommand, Guid>
{
    private readonly IBlockRepository _blockRepository;
    private readonly BlockBusinessRules _blockBusinessRules;
    

    public HardDeleteBlockCommandHandler(IBlockRepository blockRepository, BlockBusinessRules blockBusinessRules)
    {
        _blockRepository = blockRepository;
        _blockBusinessRules = blockBusinessRules;
    }

    public async Task<Guid> Handle(HardDeleteBlockCommand request, CancellationToken cancellationToken)
    {
        Block block = await _blockBusinessRules.BlockShouldBeExistInDatabase(request.Id);
        
        await _blockRepository.DeleteAsync(block, true, cancellationToken);

        return block.Id;
    }
}
