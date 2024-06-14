using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.Domain.Constants.Vehicles
{
    public static class VehicleMessages
    {
        public static class RuleMessages
        {
            public const string NoVehicleFoundBelongToIndicatedRegistrationPlate = "Bu plakaya ait bir araç bulunamadı";
            public const string RegistrationPlateCannotBeFound = "Plaka bulunamadı";
            public const string RegistrationPlateAlreadyExist = "Belirtilen plakalı araç sistemde mevcut";
            public const string AllUsersYoungerThan18 = "Plaka eklemek istediğiniz tüm kullanıcılar 18 yaşından küçük";


        }
        public static class ValidationMessages
        {
            public const string RegistraionPlateCannotBeEmpty = "Plaka boş geçilemez";
            public const string InvalidRegistrationPlate = "Geçersiz plaka bilgisi";
            public const string InvalidProvincePart = "Geçersiz il bölümü";
            public const string VehicleTypeCannotBeEmpty = "Lütfen araç tipini seçiniz";
            public const string InvalidVehicleType = "Geçersiz araç türü";

        }
    }
}
