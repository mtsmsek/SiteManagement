using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SiteManagement.Application.Abstractions.Auth;
using SiteManagement.Application.Abstractions.Email;
using SiteManagement.Application.Abstractions.Persistence;
using SiteManagement.Infrastructure.Auth;
using SiteManagement.Infrastructure.Identity;
using EmailVO = SiteManagement.Domain.Residency.ValueObjects.Email;
using SiteManagement.Domain.Billing;
using SiteManagement.Domain.Billing.ValueObjects;
using SiteManagement.Domain.Messaging;
using SiteManagement.Domain.Property;
using SiteManagement.Domain.Property.ValueObjects;
using SiteManagement.Domain.Residency;
using SiteManagement.Domain.Residency.ValueObjects;
using SiteManagement.Domain.Shared.ValueObjects;
using SiteManagement.Domain.Tenancy;
using SiteManagement.Domain.Tenancy.ValueObjects;

namespace SiteManagement.Infrastructure.Persistence.Seed;

/// <summary>
/// Optional demo data seeder. Builds a ready-to-explore set of demo state the
/// first time the database is empty — one occupied site, three residents (each
/// with a welcome email queued through the same path the production flow uses),
/// an open dues period with one paid and two unpaid items, and one admin-started
/// conversation. Idempotent by an "is there any site yet?" check so it is safe
/// to run on every startup; toggled by <c>Demo:SeedOnStartup</c>.
/// </summary>
/// <remarks>
/// Goes through the same domain factories the command handlers use, plus
/// <see cref="IUnitOfWork.SaveChangesAsync"/>, so domain events fire (an
/// assignment's <c>ResidentAssignedToApartment</c> still flips the apartment
/// to occupied through the existing handler) and the outbox stays consistent.
/// Skips MediatR on purpose: avoids the AuthorizationBehavior at startup,
/// where there is no authenticated principal.
/// </remarks>
public sealed class DemoSeeder
{
    private const string SiteName = "Lavanta Konutları";
    private const string SiteAddress = "Cumhuriyet Cd. No: 27, Bahçelievler / İstanbul";
    private const string BlockNameValue = "A";
    private const decimal DuesPerApartment = 750m;
    private const int DemoYear = 2026;
    private const int DemoMonth = 5;

    /// <summary>Specs for the three apartments laid out for the demo.</summary>
    private static readonly DemoApartmentSpec[] Apartments =
    [
        new(Number: 1, Floor: 1, Type: "1+1"),
        new(Number: 2, Floor: 1, Type: "2+1"),
        new(Number: 3, Floor: 2, Type: "2+1"),
    ];

    /// <summary>Specs for the three residents — TCs are real checksum-valid samples.</summary>
    private static readonly DemoResidentSpec[] Residents =
    [
        new("10000000146", "Selin", "Yılmaz", "selin.yilmaz@demo.local", "05551110011", TenantType.Owner, ApartmentIndex: 0),
        new("12345678950", "Mert", "Kaya", "mert.kaya@demo.local", "05551110022", TenantType.Tenant, ApartmentIndex: 1),
        new("22222222220", "Ezgi", "Demir", "ezgi.demir@demo.local", "05551110033", TenantType.Tenant, ApartmentIndex: 2),
    ];

    /// <summary>Seeds the demo state if no site exists yet. Idempotent.</summary>
    public async Task SeedAsync(
        IServiceProvider services,
        ILogger logger,
        CancellationToken ct = default)
    {
        using var scope = services.CreateScope();
        var sp = scope.ServiceProvider;
        var dbContext = sp.GetRequiredService<AppDbContext>();

        if (await dbContext.Sites.IgnoreQueryFilters().AnyAsync(ct))
        {
            logger.LogInformation("Demo seed skipped: site data already present.");
            return;
        }

        var siteRepo = sp.GetRequiredService<ISiteRepository>();
        var residentRepo = sp.GetRequiredService<IResidentRepository>();
        var assignmentRepo = sp.GetRequiredService<IApartmentAssignmentRepository>();
        var duesRepo = sp.GetRequiredService<IDuesPeriodRepository>();
        var conversationRepo = sp.GetRequiredService<IConversationRepository>();
        var unitOfWork = sp.GetRequiredService<IUnitOfWork>();
        var userAuth = sp.GetRequiredService<IUserAuthService>();
        var passwords = sp.GetRequiredService<IPasswordGenerator>();
        var emailSender = sp.GetRequiredService<IEmailSender>();
        var timeProvider = sp.GetRequiredService<TimeProvider>();

        logger.LogInformation("Demo seed starting (1 site + {ApartmentCount} apartments + {ResidentCount} residents)...",
            Apartments.Length, Residents.Length);

        var site = await SeedSiteAsync(siteRepo, unitOfWork, ct);
        var apartmentIds = ResolveApartmentIds(site);

        var residents = await SeedResidentsAsync(
            residentRepo, unitOfWork, userAuth, passwords, emailSender, ct);

        await SeedAssignmentsAsync(assignmentRepo, unitOfWork, apartmentIds, residents, ct);

        await SeedDuesPeriodAsync(duesRepo, unitOfWork, site.Id, apartmentIds, residents, ct);

        var adminUserId = await ResolveBootstrapAdminUserIdAsync(sp, ct);
        await SeedConversationAsync(conversationRepo, unitOfWork, residents[0].ResidentId, adminUserId, timeProvider, ct);

        logger.LogInformation("Demo seed complete.");
    }

    private static async Task<Site> SeedSiteAsync(
        ISiteRepository siteRepo, IUnitOfWork unitOfWork, CancellationToken ct)
    {
        var site = Site.Create(SiteName, SiteAddress);
        var block = site.AddBlock(BlockName.From(BlockNameValue));

        foreach (var spec in Apartments)
        {
            block.AddApartment(Apartment.Create(
                ApartmentNumber.From(spec.Number),
                Floor.From(spec.Floor),
                ApartmentType.From(spec.Type)));
        }

        await siteRepo.AddAsync(site, ct);
        await unitOfWork.SaveChangesAsync(ct);
        return site;
    }

    private static List<Guid> ResolveApartmentIds(Site site)
    {
        var block = site.Blocks.Single();
        return block.Apartments.OrderBy(a => a.Number.Value).Select(a => a.Id).ToList();
    }

    private static async Task<List<SeededResident>> SeedResidentsAsync(
        IResidentRepository residentRepo,
        IUnitOfWork unitOfWork,
        IUserAuthService userAuth,
        IPasswordGenerator passwords,
        IEmailSender emailSender,
        CancellationToken ct)
    {
        var seeded = new List<SeededResident>(Residents.Length);

        foreach (var spec in Residents)
        {
            var fullName = FullName.Create(spec.FirstName, spec.LastName);
            var email = EmailVO.From(spec.Email);
            var resident = Resident.Create(
                TcNo.From(spec.TcNo),
                fullName,
                email,
                PhoneNumber.From(spec.Phone));

            await residentRepo.AddAsync(resident, ct);
            await unitOfWork.SaveChangesAsync(ct);

            var password = passwords.Generate();
            var displayName = fullName.ToString();

            await userAuth.RegisterResidentUserAsync(resident.Id, email.Value, password, displayName, ct);
            // Mirror the production flow: welcome mail goes through the same sender,
            // so the demo password lands in MailHog (compose) for the operator to read.
            await emailSender.SendResidentWelcomeAsync(email.Value, displayName, password, ct);

            seeded.Add(new SeededResident(resident.Id, spec.TenantType, spec.ApartmentIndex));
        }

        return seeded;
    }

    private static async Task SeedAssignmentsAsync(
        IApartmentAssignmentRepository assignmentRepo,
        IUnitOfWork unitOfWork,
        IReadOnlyList<Guid> apartmentIds,
        IReadOnlyList<SeededResident> residents,
        CancellationToken ct)
    {
        var startDate = new DateOnly(DemoYear, 1, 1);
        foreach (var resident in residents)
        {
            var assignment = ApartmentAssignment.Assign(
                apartmentIds[resident.ApartmentIndex],
                resident.ResidentId,
                resident.TenantType,
                startDate);
            await assignmentRepo.AddAsync(assignment, ct);
        }

        // Domain events fire from SaveChangesAsync: each assignment's
        // ResidentAssignedToApartment flips its apartment to Occupied through the
        // existing Property-side handler, in the same UoW pass.
        await unitOfWork.SaveChangesAsync(ct);
    }

    private static async Task SeedDuesPeriodAsync(
        IDuesPeriodRepository duesRepo,
        IUnitOfWork unitOfWork,
        Guid siteId,
        IReadOnlyList<Guid> apartmentIds,
        IReadOnlyList<SeededResident> residents,
        CancellationToken ct)
    {
        var period = DuesPeriod.Open(siteId, BillingMonth.Of(DemoYear, DemoMonth), Money.Of(DuesPerApartment));
        await duesRepo.AddAsync(period, ct);
        await unitOfWork.SaveChangesAsync(ct);

        // Distribute one item per assigned apartment, mirroring DistributeDues.
        var addedItems = new List<DuesItem>(residents.Count);
        foreach (var resident in residents)
        {
            var item = period.AddItemFor(apartmentIds[resident.ApartmentIndex], resident.ResidentId);
            unitOfWork.MarkAsAdded(item);
            addedItems.Add(item);
        }

        // First resident has settled; the other two still owe so the resident
        // portal has unpaid items to demonstrate the pay-by-card flow.
        period.MarkItemPaid(addedItems[0].Id);

        await unitOfWork.SaveChangesAsync(ct);
    }

    private static async Task SeedConversationAsync(
        IConversationRepository conversationRepo,
        IUnitOfWork unitOfWork,
        Guid residentId,
        Guid adminUserId,
        TimeProvider timeProvider,
        CancellationToken ct)
    {
        var conversation = Conversation.Start(
            residentId,
            subject: "Hoş geldiniz",
            openerRole: MessageSenderRole.Admin,
            openerUserId: adminUserId,
            body: "Lavanta Konutları'na hoş geldiniz! Sorularınız için bu kanaldan iletişime geçebilirsiniz.",
            sentAtUtc: timeProvider.GetUtcNow().UtcDateTime);

        await conversationRepo.AddAsync(conversation, ct);
        await unitOfWork.SaveChangesAsync(ct);
    }

    /// <summary>
    /// Looks up the bootstrap admin user (seeded by <see cref="IdentitySeeder"/>)
    /// so the demo conversation has a real, non-empty sender id matching the
    /// admin who can later "reply" through the UI.
    /// </summary>
    private static async Task<Guid> ResolveBootstrapAdminUserIdAsync(IServiceProvider sp, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        var userManager = sp.GetRequiredService<UserManager<AppUser>>();
        var adminOptions = sp.GetRequiredService<IOptions<AdminBootstrapOptions>>().Value;
        var email = adminOptions.BootstrapEmail
            ?? throw new InvalidOperationException("Demo seed requires Admin:BootstrapEmail to be configured.");
        var admin = await userManager.FindByEmailAsync(email)
            ?? throw new InvalidOperationException(
                $"Demo seed requires the bootstrap admin '{email}' to exist; run IdentitySeeder first.");
        return admin.Id;
    }

    /// <summary>Static spec for a demo apartment.</summary>
    private sealed record DemoApartmentSpec(int Number, int Floor, string Type);

    /// <summary>Static spec for a demo resident with its assignment shape.</summary>
    private sealed record DemoResidentSpec(
        string TcNo,
        string FirstName,
        string LastName,
        string Email,
        string Phone,
        TenantType TenantType,
        int ApartmentIndex);

    /// <summary>Captured resident id after persistence — paired back to its target apartment.</summary>
    private sealed record SeededResident(Guid ResidentId, TenantType TenantType, int ApartmentIndex);
}
