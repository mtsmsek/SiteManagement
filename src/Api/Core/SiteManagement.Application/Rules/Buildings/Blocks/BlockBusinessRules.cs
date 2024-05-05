using SiteManagement.Application.CrossCuttingConcerns.Exceptions.Types;
using SiteManagement.Application.Rules.Commons;
using SiteManagement.Application.Services.Repositories.Buildings;
using SiteManagement.Domain.Constants.Buildings.Blocks;
using SiteManagement.Domain.Entities.Buildings;
using SiteManagement.Domain.Utulity;

namespace SiteManagement.Application.Rules.Buildings.Blocks
{
    public class BlockBusinessRules : BaseBusinessRules
    {
        private readonly IBlockRepository _blockRepository;

        public BlockBusinessRules(IBlockRepository blockRepository)
        {
            _blockRepository = blockRepository;
        }

        public async Task BlockNameCannotBeDublicateWhenAddOrUpdate(string name)
        {

            var isUnique = await _blockRepository.IsBlockNameUnique(name);

            if (!isUnique)
                throw new BusinessException(BlockMessages.RuleMessages.BlockNameAlreadyExist);
        }

        public  async Task<Block> BlockShouldBeExistInDatabase(Guid id)
        {
            var block = await _blockRepository.IsBlockExist(id);

            Ensure.NotNull(block,
                            new BusinessException(BlockMessages.RuleMessages.BlocIsNotExist));

            return block;

        }
        public async Task<Block> BlockShouldBeExistInDatabase(string name)
        {
            var block = await _blockRepository.IsBlockExist(name);

            Ensure.NotNull(block,
                            new BusinessException(BlockMessages.RuleMessages.BlocIsNotExist));

            return block;


        }

     
    }
}
