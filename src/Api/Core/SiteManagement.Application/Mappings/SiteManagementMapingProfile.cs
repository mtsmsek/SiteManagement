﻿using AutoMapper;
using SiteManagement.Application.Features.Commands.Buildings.Blocks.CreateBlock;
using SiteManagement.Application.Features.Commands.Buildings.Blocks.UpdateBlock.UpdateBlockName;
using SiteManagement.Application.Features.Queries.Blocks.GetBlockDetailByName;
using SiteManagement.Application.Features.Queries.Blocks.GetListAllBlocks;
using SiteManagement.Application.Pagination.Responses;
using SiteManagement.Domain.Entities.Buildings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.Application.Mappings;

public class SiteManagementMapingProfile : Profile
{
    public SiteManagementMapingProfile()
    {
        #region Blocks 
            #region Commands
                CreateMap<Block, CreateBlockCommand>().ReverseMap();
                CreateMap<Block, UpdateBlockNameCommand>().ReverseMap();
        #endregion
            #region Queries
                CreateMap<Block, GetListAllBlockResponse>().ReverseMap();
                CreateMap<PagedViewModel<Block>, PagedViewModel<GetListAllBlockResponse>>().ReverseMap();
                CreateMap<Block, GetBlockDetailByNameResponse>().ReverseMap();
        #endregion
        #endregion
    }
}
