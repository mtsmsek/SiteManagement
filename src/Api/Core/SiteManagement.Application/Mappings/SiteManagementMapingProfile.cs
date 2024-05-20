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
using SiteManagement.Application.Features.Commands.Payments.CreatePayment;
using SiteManagement.Application.Features.Commands.Payments.UpdatePayment;
using SiteManagement.Application.Features.Commands.Residents.CreateResident;
using SiteManagement.Application.Features.Commands.Residents.Login;
using SiteManagement.Application.Features.Commands.Residents.UpdateResident.UpdateEmail;
using SiteManagement.Application.Features.Commands.Residents.UpdateResident.UpdateInformation;
using SiteManagement.Application.Features.Commands.Security.OperationClaims.CreateOperationClaim;
using SiteManagement.Application.Features.Commands.Security.OperationClaims.UpdateOperationClaim;
using SiteManagement.Application.Features.Commands.VehicleResident.CreateVehicleResident;
using SiteManagement.Application.Features.Commands.Vehicles.CreateVehicle;
using SiteManagement.Application.Features.Commands.Vehicles.UpdateVehicle;
using SiteManagement.Application.Features.Queries.Apartments.GetListAllApartmentsByBlock;
using SiteManagement.Application.Features.Queries.Apartments.GetListApartmentsByBlockName;
using SiteManagement.Application.Features.Queries.Apartments.GetListApartmentsByStatus;
using SiteManagement.Application.Features.Queries.Apartments.GetListApartmentsInBlockByStatus;
using SiteManagement.Application.Features.Queries.Blocks.GetBlockDetailByName;
using SiteManagement.Application.Features.Queries.Blocks.GetListAllBlocks;
using SiteManagement.Application.Features.Queries.Invoices.GetListApartmentBillsByMonth;
using SiteManagement.Application.Features.Queries.Messaages.GetResidentMessages;
using SiteManagement.Application.Features.Queries.Payments.GetListResidentPayments;
using SiteManagement.Application.Features.Queries.Residents.GetListAllResidents;
using SiteManagement.Application.Features.Queries.Residents.GetListResidentByApartmentNumberAndBlockName;
using SiteManagement.Application.Features.Queries.Residents.GetListResidentByBlockName;
using SiteManagement.Application.Features.Queries.Residents.GetListResidentsByVehicle;
using SiteManagement.Application.Features.Queries.Residents.GetResidentByIdenticalNumber;
using SiteManagement.Application.Features.Queries.Residents.GetResidentsByFullName;
using SiteManagement.Application.Features.Queries.ResidentVehicles.GetListResidentVehicles;
using SiteManagement.Application.Features.Queries.Security.OperationClaims.GetListAllOperationClaims;
using SiteManagement.Application.Features.Queries.Vehicles.GetListVehicles;
using SiteManagement.Application.Features.Queries.Vehicles.GetVehicleByRegistrationPlate;
using SiteManagement.Application.Mappings.Resolvers;
using SiteManagement.Application.Pagination.Responses;
using SiteManagement.Domain.Entities.Buildings;
using SiteManagement.Domain.Entities.Invoices;
using SiteManagement.Domain.Entities.Payments;
using SiteManagement.Domain.Entities.Residents;
using SiteManagement.Domain.Entities.Security;
using SiteManagement.Domain.Entities.Vehicles;
using SiteManagement.Domain.Enumarations.Invoices;
using SiteManagement.Domain.Enumarations.Vehicles;

namespace SiteManagement.Application.Mappings;

public class SiteManagementMapingProfile : Profile
{
    public SiteManagementMapingProfile()
    {
        #region Apartments
        #region Commands
        CreateMap<CreateApartmentCommand, Apartment>()
            .ForMember(dest => dest.ApartmentType, opt => opt.MapFrom<ApartmentTypeResolver>());
        CreateMap<Apartment, CreateApartmentResponse>();

        CreateMap<Apartment, ChangeTenantStatusCommand>().ReverseMap();
        CreateMap<Apartment, ChangeResidentStatusCommand>().ReverseMap();
        #endregion  
        #region Queries 
        CreateMap<Apartment, GetListAllApartmentsByBlockResponse>()
          .ForMember(destinationMember: x => x.BlockName, memberOptions: y => y.MapFrom(x => x.Block.Name));
        CreateMap<PagedViewModel<Apartment>, PagedViewModel<GetListAllApartmentsByBlockResponse>>();

        CreateMap<Apartment, GetListApartmentsByBlockNameResponse>();
        CreateMap<PagedViewModel<Apartment>, PagedViewModel<GetListApartmentsByBlockNameResponse>>();

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
        #region Invoices
        #region Bills
        #region Commands
        //Create Functions
        CreateMap<CreateBillCommand, Bill>().ForMember(dest => dest.Type, source => source.MapFrom<BillTypeResolverForCreate>());
        CreateMap<Bill, CreateBulkBillsResponse>().ReverseMap();
        CreateMap<List<Bill>, PagedViewModel<CreateBulkBillsResponse>>().ForMember(dest => dest.Results, source => source.MapFrom(x => x.ToList()));


        //Update Functions

        //todo -- make an interface for class which include BillType
        //todo -- create interface for all class which include enumaration type or update all mappings like .MapFrom<Month>();
        CreateMap<UpdateBillCommand, Bill>().ForMember(dest => dest.Type, source => source.MapFrom<BillTypeResolverForUpdate>())
                                            .ForMember(dest => dest.Month, source => source.MapFrom<Month>(x => x.Month));
        CreateMap<Bill, UpdateBillResponse>().ReverseMap();
        #endregion
        #region Queries
        CreateMap<PagedViewModel<Bill>, PagedViewModel<GetListApartmentBillsResponse>>();
        CreateMap<Bill, GetListApartmentBillsResponse>()
            .ForMember(dest => dest.Period, source => source.MapFrom(x => x.Period))
            .ForMember(dest => dest.BillType, source => source.MapFrom(x => x.Type.Name))
            .ForMember(dest => dest.BlockName, source => source.MapFrom(x => x.Apartment.Block.Name))
            .ForMember(dest => dest.ApartmentNumber, source => source.MapFrom(x => x.Apartment.ApartmentNumber));
        #endregion
        #endregion

        #endregion
        #region Messages
        #region Commands
        CreateMap<Message, SendMessageCommand>().ReverseMap();
        CreateMap<Message, SendMessageResponse>().ReverseMap();
        #endregion
        #region Queries
        CreateMap<Message, GetResidentMessagesResponse>()
            .ForMember(dest => dest.SenderName, source => source.MapFrom(x => x.Sender.FirstName + x.Sender.LastName))
            .ForMember(dest => dest.Message, source => source.MapFrom(x => x.Text));

        CreateMap<PagedViewModel<Message>, PagedViewModel<GetResidentMessagesResponse>>();


        #endregion
        #endregion
        #region Residents
        #region Commands
        //Create Resident
        CreateMap<CreateResidentCommand, Resident>().ForMember(dest => dest.BirthDate, source =>
        {
            source.MapFrom(x => new DateTime(x.BirthYear, x.BirthMonth, x.BirthDay));
        });
        CreateMap<Resident, CreateResidentResponse>().ForMember(dest => dest.Password, source => source.Ignore());

        //Login
        CreateMap<Resident, ResidentLoginCommand>().ReverseMap();
        CreateMap<Resident, ResidentLoginCommandResponse>().ReverseMap();

        //Update Information
        CreateMap<UpdateResidentCommand, Resident>()
            .ForMember(dest => dest.BirthDate, source => source.MapFrom(x => new DateTime(x.BirthYear, x.BirthMonth, x.BirthDay)));
           
        CreateMap<Resident, UpdateResidentResponse>().ReverseMap();

        //Update Email
        CreateMap<Resident, UpdateResidentEmailCommand>().ReverseMap();
        CreateMap<Resident, UpdateResidentEmailResponse>().ReverseMap();
        #endregion
        #region Queries

        //GetListAllResidents
        CreateMap<Resident, GetListAllResidentsResponse>()
            .ForMember(dest => dest.BlockName, source => source.MapFrom(x => x.Apartment.Block.Name))
            .ForMember(dest => dest.ApartmentNumber, source => source.MapFrom(x => x.Apartment.ApartmentNumber))
            .ForMember(dest => dest.FloorNumber, source => source.MapFrom(x => x.Apartment.FloorNumber));

        CreateMap<PagedViewModel<Resident>, PagedViewModel<GetListAllResidentsResponse>>();

        //GetListResidentsByApartmentNumberAndBlockName

        CreateMap<Resident, GetListResidentsByApartmentNumberAndBlockNameResponse>()
            .ForMember(dest => dest.FloorNumber, source => source.MapFrom(x => x.Apartment.FloorNumber))
            .ForMember(dest => dest.ApartmentNumber, source => source.MapFrom(x => x.Apartment.ApartmentNumber))
            .ForMember(dest => dest.BlockName, source => source.MapFrom(x => x.Apartment.Block.Name));

        CreateMap<PagedViewModel<Resident>, PagedViewModel<GetListResidentsByApartmentNumberAndBlockNameResponse>>();

        //GetListResidentByBlockName
        CreateMap<Resident, GetListResidentsByBlockNameResponse>()
            .ForMember(dest => dest.BlockName, source => source.MapFrom(x => x.Apartment.Block.Name))
            .ForMember(dest => dest.FloorNumber, source => source.MapFrom(x => x.Apartment.FloorNumber))
            .ForMember(dest => dest.ApartmentNumber, source => source.MapFrom(x => x.Apartment.ApartmentNumber));

        CreateMap<PagedViewModel<Resident>, PagedViewModel<GetListResidentsByBlockNameResponse>>();

        //GetListResidentByVehicleRegistrationPlate
        CreateMap<Resident, GetListResidentsByVehicleResponse>()
            .ForMember(dest => dest.BlockName, source => source.MapFrom(x => x.Apartment.Block.Name))
            .ForMember(dest => dest.ApartmentNumber, source => source.MapFrom(x => x.Apartment.ApartmentNumber))
            .ForMember(dest => dest.FloorNumber, source => source.MapFrom(x => x.Apartment.FloorNumber));

        CreateMap<PagedViewModel<Resident>, PagedViewModel<GetListResidentsByVehicleResponse>>();


        //GetResidentByIdencticalNumber
        CreateMap<Resident, GetResidentByIdenticalNumberResponse>()
            .ForMember(dest => dest.BlockName, source => source.MapFrom(x => x.Apartment.Block.Name))
            .ForMember(dest => dest.ApartmentNumber, source => source.MapFrom(x => x.Apartment.ApartmentNumber))
            .ForMember(dest => dest.FloorNumber, source => source.MapFrom(x => x.Apartment.FloorNumber));

        //GetResidentsByFullName
        CreateMap<Resident, GetResidentsByFullNameResponse>()
            .ForMember(dest => dest.BlockName, source => source.MapFrom(x => x.Apartment.Block.Name))
            .ForMember(dest => dest.ApartmentNumber, source => source.MapFrom(x => x.Apartment.ApartmentNumber))
            .ForMember(dest => dest.FloorNumber, source => source.MapFrom(x => x.Apartment.FloorNumber));

        CreateMap<PagedViewModel<Resident>, PagedViewModel<GetResidentsByFullNameResponse>>();

        #endregion

        #endregion
        #region Vehicles
        #region Commands
        //Create
        CreateMap<CreateVehicleCommand, Vehicle>().ForMember(dest => dest.VehicleType, source => 
        source.MapFrom(x => VehicleType.FromValue(x.VehicleType)!));
        CreateMap<Vehicle, CreateVehicleResponse>().ReverseMap();

        //Update
        CreateMap<UpdateVehicleCommand, Vehicle>().ForMember(dest => dest.VehicleType, source => source.MapFrom(x => VehicleType.FromValue(x.VehicleType)!));
            
        CreateMap<Vehicle, UpdateVehicleCommandResponse>().ForMember(dest => dest.VehicleType, source => source.MapFrom(x => x.VehicleType.Name));

        #endregion

        #region Queries
        //GetListVehicleResponse
        CreateMap<Vehicle, GetListAllVehiclesResponse>();
        CreateMap<PagedViewModel<Vehicle>, PagedViewModel<GetListAllVehiclesResponse>>();


        //GetVehicleByRegistrationPlate
        CreateMap<Vehicle, GetVehicleByRegistrationPlateResponse>();

        #endregion
        #endregion
        #region ResidentVehicle
        #region Commands
        CreateMap<ResidentVehicle, CreateResidentVehicleCommand>().ReverseMap();
        #endregion
        #region Queries
        CreateMap<ResidentVehicle, GetListResidentVehiclesResponse>()
            .ForMember(dest => dest.VehicleRegistrationPlate, source => source.MapFrom(x => x.Vehicle.VehicleRegistrationPlate))
            .ForMember(dest => dest.VehicleType, source => source.MapFrom(x => x.Vehicle.VehicleType));
        CreateMap<PagedViewModel<ResidentVehicle>, PagedViewModel<GetListResidentVehiclesResponse>>();

        #endregion
        #endregion
        #region OperationClaim
        #region Commands
        //Create
        CreateMap<CreateOperationClaimCommand, OperationClaim>();
        CreateMap<OperationClaim, CreateOperationClaimResponse>();

        //Update
        CreateMap<UpdateOperationClaimCommand, OperationClaim>();
        CreateMap<OperationClaim, UpdateOperationClaimResponse>();
        #endregion
        #region Queries
        //GetListAllOperationClaims
        CreateMap<PagedViewModel<OperationClaim>, PagedViewModel<GetListAllOperationClaimsResponse>>();
        CreateMap<OperationClaim, GetListAllOperationClaimsResponse>();

        //GetOperationClaimByName
        CreateMap<OperationClaim, GetListAllOperationClaimsResponse>();
        #endregion
        #endregion
        #region Payments
        //Create
        CreateMap<CreatePaymentCommand, Payment>();

        //Update
        CreateMap<UpdatePaymentCommand, Payment>();
        CreateMap<Payment, UpdatePaymentResponse>();

        //GetListResidentPayments
        CreateMap<Payment, GetListResidentPaymentsResponse>().
            ForMember(dest => dest.BillType, source => source.MapFrom(x => x.Bill.Type.Name))
            .ForMember(dest => dest.Fee, source => source.MapFrom(x => x.Bill.Fee))
            .ForMember(dest => dest.PersonWhoPaid, source => source.MapFrom(x => $"{x.Resident.FirstName} {x.Resident.LastName}"));


        #endregion

       
    }
}

