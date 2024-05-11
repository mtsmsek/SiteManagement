using Microsoft.Extensions.DependencyInjection;
using SiteManagement.Application.Features.Commands.Invoices.Bills.CreateBill;
using SiteManagement.Application.Features.Commands.Invoices.Bills.CreateBulkBills;
using SiteManagement.Application.Features.Commands.Invoices.Bills.DeleteBill.HardDelete;
using SiteManagement.Application.Features.Commands.Invoices.Bills.UpdateBill;
using SiteManagement.XUnitTests.Application.Mock.FakeDatas.Invoices;

namespace SiteManagement.XUnitTests.Application.DependencyResolvers.Invoices;

public static class BillServiceRegistration
{
    public static void AddBillServices(this IServiceCollection services)
    {
        //Fake data
        services.AddTransient<BillFakeDatas>();
        //Create Bills
        services.AddTransient<CreateBillCommand>();
        services.AddTransient<CreateBillValidator>();

        //Create Bulk Bills
        services.AddTransient<CreateBulkBillsCommand>();

        //Hard Delete Bills
        services.AddTransient<HardDeleteBillCommand>();

        //Update Bills
        services.AddTransient<UpdateBillCommand>();
        services.AddTransient<UpdateBillValidatior>();

    }
}
