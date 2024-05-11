using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.Domain.Constants.Invoices.Bills;

public static class BillsMessages
{
    public static class RuleMessages
    {
        public const string ForTheSamePeriodAndApartmentCanIncludeOneBillType = "Aynı dönem için bir fatura türüne ait en fazla bir fatura kesebilirsiniz.";
        public const string BillCannotBeFoundInDb = "Aradığınız fatura veri tabanında mevcut değil";

    }
    public static class ValidationMessages
    {
        public const string BillFeeCannotBeLessThanOrEqualToZero = "Fatura ücreti 0'dan büyük olmalıdır.";

        public const string BillTypeCannotBeNull = "Fatura tipi boş bırakılamaz";
        public const string InvalidBillType = "Geçersiz fatura türü";

        public const string YearCannotBeNull = "Yıl alanı boş bırakılamaz";
        public const string YearValueShouldBeLessThanOrEqualToCurrentYearValue = "Gelecek yıla ait fatura kesemezsiniz";

        public const string MonthCannotBeNull = "Ay alanı boş bırakılamaz.";
        public const string MonthValueShouldBeBetweenOneAndTwelve = "Ay değeri 1 ve 12 arasında olmalı.";
        public const string MonthValueShouldBeLessThanOrEqualToCurrentMonthValue = "Gelecek aylara ait fatura kesemezsiniz.";
    }
}
