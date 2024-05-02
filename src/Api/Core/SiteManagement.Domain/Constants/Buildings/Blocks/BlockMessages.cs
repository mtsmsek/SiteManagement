using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.Domain.Constants.Buildings.Blocks
{
    public static class BlockMessages
    {
        public static class CacheKeys
        {
            public const string CacheKey = "GetBlocks";
        }
        public static class RuleMessages
        {
            public const string BlockNameAlreadyExist = "Block name you want to add is already exist";
            public const string BlocIsNotExist = "The block you try to update cannot found!";


        }
        public static class ValidationMessages
        {
            public const string BlockNameCannotBeEmpty = "Block name must be filled";
            public const string BlockNameCannotBeLongerThanTwoCharacters = "Block ismi en fazla iki karakterden oluşabilir";
            public const string BlockNameMustStartWithALetter = "Block ismi bir harf ile başlamalıdır";
        }
        
    }
}
