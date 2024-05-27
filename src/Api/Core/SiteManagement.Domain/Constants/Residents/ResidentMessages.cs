using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.Domain.Constants.Residents
{
    public static class ResidentMessages
    {
        public static class RuleMessages
        {
            public const string ResidentAlreadyExists = "Bu kullanıcı zaten mevcut";
            public const string EmailCannotBeDuplicated = "Bu maile ait başka bir kullanıcı mevcut";
            public const string ResidentCannotBeFound = "Kullanıcı bulunamadı";

            public const string WrongPassword = "Hatalı Şifre";
            public const string NewEmailCannotBeSameWithTheOldEmail = "Yeni email'iniz eskisi ile aynı olamaz";
            public const string OldPasswordWrong = "Old password is wrong";
        }
        public static class  ValidationMessages
        {
            public const string FirstNameCannotBeEmpty = "Ad alanı boş bırakılamaz";
            public const string LastNameCannotBeEmpty = "Soyad alanı boş bırakılamaz";

            public const string BirthYearCannotBeEmpty = "Doğum yılınız boş bırakılamaz";
            public const string InvalidBirthYear= "Geçersiz doğum yılı";

            public const string BirthMonthCannotBeEmpty = "Doğum tarihi ay bölümü boş bırakılamaz";
            public const string MonthValueMustBeBetweenOneAndTwelve = "Ay değeri 1 ve 12 arasında olmalı.";

            public const string BirthDayCannotBeEmpty = "Gün alanı boş bırakılamaz";
            public const string InvalidBirthDay = "Geçersiz gün alanı";

            public const string InvalidBirthMonth = "Geçersiz doğum ayı";
            public const string EmailCannotBeEmpty = "Email alanı boş bırakılamaz";
            public const string InvalidEmail = "Geçersiz email";
            public const string IdenticalNumberMustIncludeElevenChar = "Kimlik numaranız 11 haneden oluşmalıdır";

            public const string PhoneNumberCannotBeEmpty = "Telefon numarası boş olamaz";
            public const string InvalidPhoneNumber = "Geçersiz telefon numarası.";

            //Login
            public const string EmailOrIdenticalNumberCannotBeEmpty = "Lütfen email ya da TC numaranızı giriniz";
            public const string WrongPassword = "Parola yanlış.";

            //UpdatePassword
            public const string PasswordCannotBeEmpty = "Parola alanı boş bırakılamaz";
            public const string PasswordShouldLongerThanEightCharAndLessThanOrEqualToSixTeenChar = "Paralonuz en az 8 en fazla 16 karakter uzunluğunda olmalıdır.";
            public const string PasswordShouldIncludeAtLeastOneBiggerOneNumberAndSpecialChar = "Parolanız en az bir büyük harf, bir sayı ve bir özel karakter içermelidir";



        }
    }
}
