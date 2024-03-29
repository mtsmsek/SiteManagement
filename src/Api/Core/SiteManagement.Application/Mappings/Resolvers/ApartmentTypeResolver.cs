using AutoMapper;
using SiteManagement.Application.Features.Commands.Buildings.Apartments.CreateApartment;
using SiteManagement.Domain.Entities.Buildings;
using SiteManagement.Domain.Enumarations.Buildings;

namespace SiteManagement.Application.Mappings.Resolvers;

public class ApartmentTypeResolver : IValueResolver<CreateApartmentCommand, Apartment, ApartmentType>
{
    public ApartmentType Resolve(CreateApartmentCommand source, Apartment destination, ApartmentType destMember, ResolutionContext context)
    {
        return ApartmentType.FromValue(source.ApartmentType)!;
    }
}
