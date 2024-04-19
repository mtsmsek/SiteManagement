using Bogus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SiteManagement.Application.Security.Hashing;
using SiteManagement.Domain.Entities.Buildings;
using SiteManagement.Domain.Entities.Invoices;
using SiteManagement.Domain.Entities.Residents;
using SiteManagement.Domain.Entities.Vehicles;
using SiteManagement.Domain.Enumarations.Buildings;
using SiteManagement.Domain.Enumarations.Invoices;
using SiteManagement.Domain.Enumarations.Security;
using SiteManagement.Domain.Enumarations.Vehicles;
using System.Linq;

namespace SiteManagement.Persistance.Contexts.SeedDatas;

internal class SiteManagementSeedData
{
    internal static SiteManagementApplicationContext context;
    #region Residents
    private List<Resident> GetResidents(IEnumerable<Guid> apartmentIds)
    {
        var residents = new Faker<Resident>("tr")
            .RuleFor(resident => resident.Id, faker => Guid.NewGuid())
            .RuleFor(resident => resident.CreatedDate, faker => faker.PickRandom(DateTime.Now.AddDays(-500), DateTime.Now))
            .RuleFor(resident => resident.ApartmentId, faker => faker.PickRandom(apartmentIds))
            .RuleFor(resident => resident.FirstName, faker => faker.Person.FirstName)
            .RuleFor(resident => resident.LastName, faker => faker.Person.LastName)
            .RuleFor(resident => resident.PasswordHash, faker => ReturnPasswordHash(faker.Internet.Password()))
            .RuleFor(resident => resident.PasswordSalt, faker => ReturnPasswordSalt(faker.Internet.Password()))
            .RuleFor(resident => resident.LastName, faker => faker.Person.LastName)
            .RuleFor(resident => resident.IdenticalNumber, faker => faker.Database.Random.AlphaNumeric(11))
            .RuleFor(resident => resident.PhoneNumber, faker => faker.Person.Phone)
            .RuleFor(resident => resident.Email, faker => faker.Internet.Email())
            .RuleFor(resident => resident.AuthenticatorType, faker => faker.PickRandom( AuthenticatorType.None,
                                                                                        AuthenticatorType.Otp,
                                                                                        AuthenticatorType.Email))
            .Generate(500);
        return residents;


    }
    #endregion
    #region Buildings
    private List<Apartment> GetApartments(IEnumerable<Guid> blockIds)
    {

        var apartments = new Faker<Apartment>("tr")
            .RuleFor(apartments => apartments.Id, faker => Guid.NewGuid())
            .RuleFor(apartments => apartments.CreatedDate, faker => faker.PickRandom(DateTime.Now.AddDays(-500), DateTime.Now))
            .RuleFor(apartment => apartment.BlockId, faker => faker.PickRandom(blockIds))
            .RuleFor(apartment => apartment.Status, faker => faker.PickRandom(true, false))
            .RuleFor(apartment => apartment.ApartmentType, faker => faker.PickRandom(ApartmentType.Studio,
                                                                                     ApartmentType.TwoPlusOne,
                                                                                     ApartmentType.ThreePlusOne))
            .RuleFor(apartment => apartment.ApartmentNumber, faker => faker.Rant.Random.Number(1, 60))
            .RuleFor(apartment => apartment.FloorNumber, faker => faker.Rant.Random.Number(1, 15))
            .RuleFor(apartment => apartment.IsTenant, faker => faker.PickRandom(true, false))
            .Generate(300);

        return apartments;

    }
    private List<Block> GetBlocks()
    {
        var blockSet = new List<string>();
        var blocks = new Faker<Block>("tr")
            .CustomInstantiator(faker =>
            {

                var blockName =  faker.Lorem.Letter(1);
                while (blockSet.Contains(blockName))
                {
                    blockName = faker.Lorem.Letter(1);
                }
                blockSet.Add(blockName);
                return new Block { Id = Guid.NewGuid() ,
                                  Name = blockName, 
                                  CreatedDate = DateTime.Now };
            })
            .Generate(5);
        return blocks;
    }
    #endregion
    #region Invoices
    private List<Bill> GetBills(IEnumerable<Guid> apartmentIds)
    {
        var bills = new Faker<Bill>("tr")
            .RuleFor(bill => bill.Id, faker => Guid.NewGuid())
            .RuleFor(bill => bill.CreatedDate, faker => faker.PickRandom(DateTime.Now.AddDays(-500), DateTime.Now))
            .RuleFor(bill => bill.ApartmentId, faker => faker.PickRandom(apartmentIds))
            .RuleFor(bill => bill.Type, faker => faker.PickRandom(BillType.MaintenanceFee,
                                                                  BillType.NaturalGas,
                                                                  BillType.Electricity,
                                                                  BillType.WaterBill))
            .RuleFor(bill => bill.Month, faker => faker.PickRandom(Month.January,
                                                                   Month.February,
                                                                   Month.March,
                                                                   Month.April,
                                                                   Month.May,
                                                                   Month.June,
                                                                   Month.July,
                                                                   Month.August,
                                                                   Month.September,
                                                                   Month.October,
                                                                   Month.November,
                                                                   Month.December))
            .RuleFor(bill => bill.Year, faker => faker.PickRandom<int>(DateTime.Now.AddDays(-500).Year, DateTime.Now.Year))
            .RuleFor(bill => bill.Fee, faker => faker.Rant.Random.Double(50, 3500))
            .RuleFor(bill => bill.IsPaid, faker => faker.PickRandom(true, false))
            .Generate(2000);

        return bills;
    }
    #endregion
    #region Vehicles
    private List<SiteManagement.Domain.Entities.Vehicles.Vehicle> GetVehicles()
    {
       
        var vehicles = new Faker<SiteManagement.Domain.Entities.Vehicles.Vehicle>("tr")
            .RuleFor(vehicle => vehicle.Id, faker => Guid.NewGuid())
            .RuleFor(vehicle => vehicle.CreatedDate, faker => faker.PickRandom(DateTime.Now.AddDays(-500), DateTime.Now))
            .RuleFor(vehicle => vehicle.VehicleRegistrationPlate, faker => faker.Vehicle.Vin(true))
            .RuleFor(vehicle => vehicle.VehicleType, faker => faker.PickRandom(VehicleType.Motorcycle,
                                                                               VehicleType.Car))
            .Generate(200);

        return vehicles;


    }
    private List<ResidentVehicle> GetResidentVehicles(IEnumerable<Guid> residentIds, IEnumerable<Guid> vehicleIds)
    {
        var residentVehicleSet = new HashSet<(Guid, Guid)>();
        var residentVehicles = new Faker<ResidentVehicle>("tr")
            .CustomInstantiator(faker =>
            {
                var residentId = faker.PickRandom(residentIds);
                var vehicleId = faker.PickRandom(vehicleIds);
                while (residentVehicleSet.Contains((residentId, vehicleId)))
                {
                    residentId = faker.PickRandom(residentIds);
                    vehicleId = faker.PickRandom(vehicleIds);
                }
                residentVehicleSet.Add((residentId, vehicleId));
                return new ResidentVehicle { ResidentId = residentId, VehicleId = vehicleId };
            })
            .Generate(200);

        return residentVehicles;
    }
    #endregion
    public byte[] ReturnPasswordHash(string password)
    {
        HashingHelper.CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);
        return passwordHash;
           
    }
    public byte[] ReturnPasswordSalt(string password)
    {
        HashingHelper.CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);
        return passwordSalt;

    }
    public async Task SeedAsync(IConfiguration configuration)
    {
       var dbContextBuilder = new DbContextOptionsBuilder();
        dbContextBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnectionString"));

         context = new SiteManagementApplicationContext(dbContextBuilder.Options);

        if (context.Residents.Any())
        {
            await Task.CompletedTask;
            return;
        }

        #region Blocks
        var blocks = GetBlocks();
        var blockIds = blocks.Select(block => block.Id);

        await context.Blocks.AddRangeAsync(blocks);
        #endregion
        #region Apartments
        var apartments = GetApartments(blockIds);
        var apartmentIds = apartments.Select(apartment => apartment.Id);

        await context.Apartments.AddRangeAsync(apartments);
        #endregion
        #region Bills
        var bills = GetBills(apartmentIds);
        var billIds = bills.Select(i => i.Id);

        await context.Bills.AddRangeAsync(bills);
        #endregion
        #region Residents
        var residents = GetResidents(apartmentIds);
        var residentIds = residents.Select(i => i.Id);

        await context.Residents.AddRangeAsync(residents);
        #endregion

        #region Vehicles
        var vehicles = GetVehicles();
        var vehicleIds = vehicles.Select(i => i.Id);

        await context.Vehicles.AddRangeAsync(vehicles);


        var residentVehicles = GetResidentVehicles(residentIds, vehicleIds);
        await context.ResidentVehicles.AddRangeAsync(residentVehicles);
        #endregion

        await context.SaveChangesAsync();


    }


}
