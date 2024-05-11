using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.Domain.Constants.Buildings.Apartments
{
    public static class ApartmentMessages
    {
        public static class RuleMessages
        {
            public const string ApartmentNumberCannotDuplicateForSameBlock = "Apartman numarası aynı blok için tekrar edemez.";
            public const string ApartmentCannotBeFound = "Apartman bulunamadı";

        }
        public static class ValidationMessages
        {
            public const string ApartmentNumberCannotBeLowerThanOne = "Apartman numarası 1 den başlamalıdır.";
            public const string BlockIdAndBlockNameCannotBeEmptyAtTheSameTime = "Lütfen bir blok seçiniz.";
        }
    }
}
