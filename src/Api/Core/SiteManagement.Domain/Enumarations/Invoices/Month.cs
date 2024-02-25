using SiteManagement.Domain.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.Domain.Enumarations.Invoices
{
    public class Month : Enumaration<Month>
    {
        public static Month January = new(1, "January");
        public static Month February = new(2, "February");
        public static Month March = new(3, "March");
        public static Month April = new(4, "April");
        public static Month May = new(5, "May");
        public static Month June = new(6, "June");
        public static Month July = new(7, "July");
        public static Month August = new(8, "August");
        public static Month September = new(9, "September");
        public static Month October = new(10, "October");
        public static Month November = new(11, "November");
        public static Month December = new(12, "December");
        public Month(int value, string name) : base(value, name)
        {
        }

        public static implicit operator Month(int type)
        {
            return type switch
            {
                1 => January,
                2 => February,
                3 => March,
                4 => April,
                5 => May,
                6 => June,
                7 => July,
                8 => August,
                9 => September,
                10 => October,
                11 => November,
                12 => December
            };
        }
    }
}
