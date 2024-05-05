using AutoMapper;
using MediatR;
using SiteManagement.Application.Rules.Buildings.Blocks;
using SiteManagement.Application.Services.Repositories.Buildings;
using SiteManagement.Domain.Entities.Buildings;

namespace SiteManagement.Application.Features.Commands.Buildings.Blocks.UpdateBlock.UpdateBlockName;

public class UpdateBlockNameCommandHandler : IRequestHandler<UpdateBlockNameCommand, UpdateBlockNameResponse>
{
    private readonly IBlockRepository _blockRepository;
    private readonly IMapper _mapper;
    private readonly BlockBusinessRules _blockBusinessRules;


    public UpdateBlockNameCommandHandler(IBlockRepository blockRepository, IMapper mapper, BlockBusinessRules blockBusinessRules)
    {
        _blockRepository = blockRepository;
        _mapper = mapper;
        _blockBusinessRules = blockBusinessRules;
    }

    public async Task<UpdateBlockNameResponse> Handle(UpdateBlockNameCommand request, CancellationToken cancellationToken)
    {
        
        Block block = await _blockBusinessRules.BlockShouldBeExistInDatabase(request.Id);
        
        await _blockBusinessRules.BlockNameCannotBeDublicateWhenAddOrUpdate(request.Name);
        
        var oldName = block.Name;

        block = _mapper.Map(request, block);

        await _blockRepository.UpdateAsync(block);
        
       
        return new UpdateBlockNameResponse (oldName,request.Name, block.UpdatedDate!.Value);
     
    }
}
