using AutoMapper;
using SiteManagement.Application.Features.Commands.Buildings.Apartments.CreateApartment;
using SiteManagement.Application.Features.Commands.Buildings.Apartments.UpdateApartment.ChangeResidentStatus;
using SiteManagement.Application.Features.Commands.Buildings.Apartments.UpdateApartment.ChangeTenantStatus;
using SiteManagement.Application.Features.Commands.Buildings.Blocks.CreateBlock;
using SiteManagement.Application.Features.Commands.Buildings.Blocks.UpdateBlock.UpdateBlockName;
using SiteManagement.Application.Features.Queries.Apartments.GetListAllApartmentsByBlockId;
using SiteManagement.Application.Features.Queries.Apartments.GetListApartmentsByStatus;
using SiteManagement.Application.Features.Queries.Apartments.GetListApartmentsInBlockByStatus;
using SiteManagement.Application.Features.Queries.Blocks.GetBlockDetailByName;
using SiteManagement.Application.Features.Queries.Blocks.GetListAllBlocks;
using SiteManagement.Application.Pagination.Responses;
using SiteManagement.Domain.Entities.Buildings;
using SiteManagement.Domain.Enumarations.Buildings;
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

        #region Apartments
        #region Commands
        CreateMap<Apartment, CreateBlockCommand>().ReverseMap();
        CreateMap<Apartment, CreateApartmentResponse>().ReverseMap();

        CreateMap<Apartment, ChangeTenantStatusCommand>().ReverseMap();
        CreateMap<Apartment, ChangeResidentStatusCommand>().ReverseMap();
        #endregion
        #region Queries
        CreateMap<Apartment, GetListAllApartmentsByBlockResponse>()
          .ForMember(destinationMember: x => x.BlockName, memberOptions: y => y.MapFrom(x => x.Block.Name)).ReverseMap();
        CreateMap<PagedViewModel<Apartment>, PagedViewModel<GetListAllApartmentsByBlockResponse>>().ReverseMap();
        
        
        CreateMap<Apartment, GetListApartmentsByStatusResponse>()
          .ForMember(destinationMember: x => x.BlockName, memberOptions: y => y.MapFrom(x => x.Block.Name))
          .ForMember(destinationMember: x => x.ApartmentType, memberOptions: y => y.MapFrom(x => x.ApartmentType.Name)).ReverseMap();
        CreateMap<PagedViewModel<Apartment>, PagedViewModel<GetListApartmentsByStatusResponse>>().ReverseMap();

        CreateMap<Apartment, GetListApartmentsInBlockByStatusResponse>()
         .ForMember(destinationMember: x => x.BlockName, memberOptions: y => y.MapFrom(x => x.Block.Name))
         .ForMember(destinationMember: x => x.ApartmentType, memberOptions: y => y.MapFrom(x => x.ApartmentType.Name)).ReverseMap();
        CreateMap<PagedViewModel<Apartment>, PagedViewModel<GetListApartmentsInBlockByStatusResponse>>().ReverseMap();

        #endregion

        #endregion
    }
}
