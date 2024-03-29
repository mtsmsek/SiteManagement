using AutoMapper;
using SiteManagement.Application.Features.Commands.Invoices.Bills.CreateBill;
using SiteManagement.Application.Features.Commands.Invoices.Bills.UpdateBill;
using SiteManagement.Domain.Entities.Invoices;
using SiteManagement.Domain.Enumarations.Invoices;

namespace SiteManagement.Application.Mappings.Resolvers;

public class BillTypeResolverForCreate : IValueResolver<CreateBillCommand, Bill, BillType>
{
    public BillType Resolve(CreateBillCommand source, Bill destination, BillType destMember, ResolutionContext context)
    {
        return BillType.FromValue(source.Type)!;
    }
}
public class BillTypeResolverForUpdate : IValueResolver<UpdateBillCommand, Bill, BillType>
{
    public BillType Resolve(UpdateBillCommand source, Bill destination, BillType destMember, ResolutionContext context)
    {
        return BillType.FromValue(source.Type)!;
    }
}
