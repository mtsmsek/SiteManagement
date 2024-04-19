using Bogus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SiteManagement.Domain.Entities.Payments;
using SiteManagement.Domain.Entities.Residents;

namespace SiteManagement.Persistance.Contexts.SeedDatas;

public class PaymentsSeedData
{
    private List<Guid> _userIds;
    public PaymentsSeedData()
    {
        _userIds = new List<Guid>();

    }
    #region CreditCards
    private List<CreditCard> GetCreditCards()
    {
        var creditCards = new Faker<CreditCard>("tr")
           .RuleFor(card => card.Id, faker => Guid.NewGuid())
           .RuleFor(card => card.CreatedDate, faker => faker.PickRandom(DateTime.Now.AddDays(-500), DateTime.Now))
           .RuleFor(card => card.NameOnCard, faker => faker.Person.FullName)
           .RuleFor(card => card.CardNumber, faker => faker.Finance.CreditCardNumber())
           .RuleFor(card => card.CVCNumber, faker => faker.Finance.CreditCardCvv())
           .RuleFor(card => card.ExpireDate, faker => faker.Date.FutureDateOnly().Month.ToString().PadLeft(2, '0') + faker.Date.FutureDateOnly().Year.ToString().TakeLast(2))
           .RuleFor(card => card.Amount, faker => faker.Finance.Amount(0, 450000))
           .Generate(1500);
        return creditCards;

    }
    private List<Payment> GetPayments(IEnumerable<Guid> userGuids, IEnumerable<Guid> billGuids, IEnumerable<Guid> apartmentGuids)
    {
        var payments = new Faker<Payment>("tr")
            .RuleFor(payment => payment.Id, faker => Guid.NewGuid())
            .RuleFor(payment => payment.CreatedDate, faker => faker.PickRandom(DateTime.Now.AddDays(-500), DateTime.Now))
            .RuleFor(payment => payment.UserId, faker => faker.PickRandom(userGuids))
            .RuleFor(payment => payment.BillId, faker => faker.PickRandom(billGuids))
            .RuleFor(payment => payment.ApartmentId, faker => faker.PickRandom(apartmentGuids))
            .Generate(1500);
        return payments;
    }

    #endregion
    public async Task SeedAsync(IConfiguration configuration)
    {
        var dbContextBuilder = new DbContextOptionsBuilder();
        dbContextBuilder.UseNpgsql(configuration.GetConnectionString("PostgreConnectionString"));

        var context = new SiteManagementPaymentsContext(dbContextBuilder.Options);

        if (context.CreditCards.Any())
        {
            await Task.CompletedTask;
            return;
        }

        #region Credit Cards
        await context.CreditCards.AddRangeAsync(GetCreditCards());
        #endregion
        #region GetPayments 
        var userGuids = await SiteManagementSeedData.context.Users.Select(x => x.Id).ToListAsync();
        var billGuids = await SiteManagementSeedData.context.Bills.Select(x => x.Id).ToListAsync();
        var apartmentGuids = await SiteManagementSeedData.context.Apartments.Select(x => x.Id).ToListAsync();
        var payments = GetPayments(userGuids, billGuids, apartmentGuids);

        await context.AddRangeAsync(payments);
        #endregion
        await context.SaveChangesAsync();

    }
}
