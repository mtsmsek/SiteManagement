using SiteManagement.Domain.Primitives;

namespace SiteManagement.Domain.Enumarations.Buildings;

public class ApartmentType : Enumaration<ApartmentType>
{
    public static readonly ApartmentType TwoPlusOne = new(1, "2+1");
    public static readonly ApartmentType ThreePlusOne = new(1, "3+1");
    public static readonly ApartmentType Studio = new(1, "Studio");
    private ApartmentType(int value, string name) : base(value, name)
    {
    }
}
