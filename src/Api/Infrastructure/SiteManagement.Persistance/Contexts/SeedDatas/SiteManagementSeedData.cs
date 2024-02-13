using Bogus;
using SiteManagement.Domain.Entities.Buildings;
using SiteManagement.Domain.Entities.Invoices;
using SiteManagement.Domain.Entities.Residents;
using SiteManagement.Domain.Entities.Vehicles;
using SiteManagement.Domain.Enumarations.Buildings;
using SiteManagement.Domain.Enumarations.Invoices;
using SiteManagement.Domain.Enumarations.Vehicles;

namespace SiteManagement.Persistance.Contexts.SeedDatas
{
    internal class SiteManagementSeedData
    {
        //TODO  -- add all class id and create date
        //TODO  -- check uniqe datas 
        #region Residents
        private List<Resident> GetResidents(IEnumerable<Guid> apartmentIds)
        {
            var residents = new Faker<Resident>("tr")
                .RuleFor(resident => resident.ApartmentId, faker => faker.PickRandom(apartmentIds))
                .RuleFor(resident => resident.FirstName, faker => faker.Person.FirstName)
                .RuleFor(resident => resident.LastName, faker => faker.Person.LastName)
                .RuleFor(resident => resident.IdenticalNumber, faker => faker.Database.Random.AlphaNumeric(11))
                .RuleFor(resident => resident.PhoneNumber, faker => faker.Person.Phone)
                .RuleFor(resident => resident.Email, faker => faker.Internet.Email())
                .Generate(500);
                
                return residents;
                
                
        }
        #endregion
        #region Buildings
        private List<Apartment> GetApartments(IEnumerable<Guid> blockIds)
        {
            //TODO make apartment numbers and floor numbers unuqie for apartment in the same block
            var apartments = new Faker<Apartment>("tr")
                .RuleFor(apartment => apartment.BlockId, faker => faker.PickRandom(blockIds))
                .RuleFor(apartment => apartment.Status, faker => faker.PickRandom(true, false))
                .RuleFor(apartment => apartment.ApartmentType, faker => faker.PickRandom(ApartmentType.Studio,
                                                                                         ApartmentType.TwoPlusOne,
                                                                                         ApartmentType.ThreePlusOne))
                .RuleFor(apartment => apartment.ApartmentNumber, faker => faker.Rant.Random.Number(1, 60))
                .RuleFor(apartment => apartment.FloorNumber, faker => faker.Rant.Random.Number(1, 15))
                .RuleFor(apartment => apartment.IsTenant, faker => faker.PickRandom(true,false))
                .Generate(300);

            return apartments;

        }
        private List<Block> GetBlocks()
        {
            //TODO - check if letter generate same
            var blocks = new Faker<Block>()
                .RuleFor(block => block.Name, faker => faker.Lorem.Letter(1))
                .Generate(5);

            return blocks;
        }
        #endregion
        #region Invoices
        private List<Bill> GetBills(IEnumerable<Guid> apartmentIds)
        {
            var bills = new Faker<Bill>("tr")
                .RuleFor(bill => bill.ApartmentId, faker => faker.PickRandom(apartmentIds))
                .RuleFor(bill => bill.Type, faker => faker.PickRandom(BillType.MaintenanceFee,
                                                                      BillType.NaturalGas,
                                                                      BillType.Electricity,
                                                                      BillType.WaterBill))
                .RuleFor(bill => bill.Fee, faker => faker.Rant.Random.Double(50,3500))
                .RuleFor(bill => bill.IsPaid, faker => faker.PickRandom(true,false))
                .Generate(2000);

            return bills;
        }
        #endregion
        #region Vehicles
        private List<Vehicle> GetVehicles()
        {
            var vehicles = new Faker<Vehicle>("tr")
                .RuleFor(vehicle => vehicle.VehicleRegistrationPlate, faker => faker.Vehicle.Vin(true))
                .RuleFor(vehicle => vehicle.VehicleType, faker => faker.PickRandom(VehicleType.Motorcycle,
                                                                                   VehicleType.Car))
                .Generate(200);

            return vehicles;
                
                
        }
        private List<ResidentVehicle> GetResidentVehicles(IEnumerable<Guid> residentIds, IEnumerable<Guid> vehicleIds) 
        {
            var residentVehicles = new Faker<ResidentVehicle>("tr")
                .RuleFor(residentVehicle => residentVehicle.VehicleId,
                         faker => faker.PickRandom(vehicleIds))
                .RuleFor(residentVehicle => residentVehicle.ResidentId,
                         faker => faker.PickRandom(residentIds))
                .Generate(200);
                
            return residentVehicles;
        }
        #endregion
    }
}
