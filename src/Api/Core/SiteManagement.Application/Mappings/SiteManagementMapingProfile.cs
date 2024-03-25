using AutoMapper;
using SiteManagement.Application.Features.Commands.Buildings.Apartments.CreateApartment;
using SiteManagement.Application.Features.Commands.Buildings.Apartments.UpdateApartment.ChangeResidentStatus;
using SiteManagement.Application.Features.Commands.Buildings.Apartments.UpdateApartment.ChangeTenantStatus;
using SiteManagement.Application.Features.Commands.Buildings.Blocks.CreateBlock;
using SiteManagement.Application.Features.Commands.Buildings.Blocks.UpdateBlock.UpdateBlockName;
using SiteManagement.Application.Features.Commands.Invoices.Bills.CreateBill;
using SiteManagement.Application.Features.Commands.Invoices.Bills.CreateBulkBills;
using SiteManagement.Application.Features.Commands.Invoices.Bills.UpdateBill;
using SiteManagement.Application.Features.Commands.Messages.SendMessage;
using SiteManagement.Application.Features.Commands.Residents.CreateResident;
using SiteManagement.Application.Features.Commands.Residents.Login;
using SiteManagement.Application.Features.Commands.Residents.UpdateResident.UpdateEmail;
using SiteManagement.Application.Features.Commands.Residents.UpdateResident.UpdateInformation;
using SiteManagement.Application.Features.Commands.VehicleResident.CreateVehicleResident;
using SiteManagement.Application.Features.Commands.Vehicles.CreateVehicle;
using SiteManagement.Application.Features.Commands.Vehicles.UpdateVehicle;
using SiteManagement.Application.Features.Queries.Apartments.GetListAllApartmentsByBlockId;
using SiteManagement.Application.Features.Queries.Apartments.GetListApartmentsByStatus;
using SiteManagement.Application.Features.Queries.Apartments.GetListApartmentsInBlockByStatus;
using SiteManagement.Application.Features.Queries.Blocks.GetBlockDetailByName;
using SiteManagement.Application.Features.Queries.Blocks.GetListAllBlocks;
using SiteManagement.Application.Pagination.Responses;
using SiteManagement.Domain.Entities.Buildings;
using SiteManagement.Domain.Entities.Invoices;
using SiteManagement.Domain.Entities.Residents;
using SiteManagement.Domain.Entities.Vehicles;

namespace SiteManagement.Application.Mappings;

public class SiteManagementMapingProfile : Profile
{
    public SiteManagementMapingProfile()
    {
        #region Blocks 
        #region Commands
        CreateMap<Block, CreateBlockCommand>().ReverseMap();
        CreateMap<Block, UpdateBlockNameCommand>().ReverseMap();

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
        #region Invoices
        #region Bills
            #region Commands
            //Create Functions
            CreateMap<Bill, CreateBillCommand>().ReverseMap();
            CreateMap<Bill, CreateBulkBillsResponse>().ReverseMap();
            CreateMap<IEnumerable<Bill>, PagedViewModel<CreateBulkBillsResponse>>().ReverseMap();

            //Update Functions
            CreateMap<Bill, UpdateBillCommand>().ReverseMap();
            CreateMap<Bill, UpdateBillResponse>().ReverseMap();
        #endregion

        #endregion

        #endregion
        #region Messages
        #region Commands
        CreateMap<Message, SendMessageCommand>().ReverseMap();
        CreateMap<Message, SendMessageResponse>().ReverseMap();
        #endregion
        #region Queries
        #endregion
        #endregion
        #region Residents
        #region Commands
        //Create Resident
        CreateMap<CreateResidentCommand, Resident>().ReverseMap();
        CreateMap<Resident, CreateResidentResponse>().ReverseMap();

        //Login
        CreateMap<Resident, ResidentLoginCommand>().ReverseMap();
        CreateMap<Resident, ResidentLoginCommandResponse>().ReverseMap();

        //Update Information
        CreateMap<Resident, UpdateResidentCommand>().ReverseMap();
        CreateMap<Resident, UpdateResidentResponse>().ReverseMap();

        //Update Email
        CreateMap<Resident, UpdateResidentEmailCommand>().ReverseMap();
        CreateMap<Resident, UpdateResidentEmailResponse>().ReverseMap();
        #endregion
        #region Queries

        #endregion

        #endregion
        #region Vehicles
        #region Commands
        //Create
        CreateMap<Vehicle, CreateVehicleCommand>().ReverseMap();
        CreateMap<Vehicle, CreateVehicleResponse>().ReverseMap();

        //Update
        CreateMap<Vehicle, UpdateVehicleCommand>().ReverseMap();
        CreateMap<Vehicle, UpdateVehicleCommandResponse>().ReverseMap();

        #endregion

        #region Queries
        #endregion
        #endregion
        #region VehicleResident
        #region Commands
        CreateMap<ResidentVehicle, CreateResidentVehicleCommand>().ReverseMap();
        #endregion
        #region Queries
        #endregion
        #endregion
    }
}
