using AutoMapper;
using SiteManagement.Application.Features.Commands.Buildings.Blocks.CreateBlock;
using SiteManagement.Domain.Entities.Buildings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.Application.Mappings
{
    public class SiteManagementMapingProfile : Profile
    {
        public SiteManagementMapingProfile()
        {
            #region Blocks 
                #region CreateBlock
                    CreateMap<Block, CreateBlockCommand>().ReverseMap();
                #endregion
            #endregion
        }
    }
}
