using SiteManagement.Application.CrossCuttingConcerns.Exceptions.Types;
using SiteManagement.Application.Rules.Commons;
using SiteManagement.Application.Services.Repositories.Buildings;
using SiteManagement.Domain.Constants.Buildings.Blocks;
using SiteManagement.Domain.Entities.Buildings;
using SiteManagement.Domain.Utulity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.Application.Rules.Buildings.Blocks
{
    public class BlockBusinessRules : BaseBusinessRules
    {
        private readonly IBlockRepository _blockRepository;

        public BlockBusinessRules(IBlockRepository blockRepository)
        {
            _blockRepository = blockRepository;
        }

        public async Task BlockNameCannotBeDublicateWhenInserted(string name)
        {
           var isUnique =  await _blockRepository.IsBlockNameUnique(name);

            if (!isUnique)
                throw new BusinessException(BlockMessages.RuleMessages.BlockNameAlreadyExist);
        }

        public async Task<Block> BlockShouldBeExistInDatabase(Guid id)
        {
           var block =  await _blockRepository.IsBlockExist(id);
            
            Ensure.NotNull(block,
                            new BusinessException(BlockMessages.RuleMessages.BlocIsNotExist));

            return block;

        }
    }
}
