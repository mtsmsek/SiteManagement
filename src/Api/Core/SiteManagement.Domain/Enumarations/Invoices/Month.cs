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
        public static Month February = new(1, "February");
        public static Month March = new(1, "March");
        public static Month April = new(1, "April");
        public static Month May = new(1, "May");
        public static Month June = new(1, "June");
        public static Month July = new(1, "July");
        public static Month September = new(1, "September");
        public static Month October = new(1, "October");
        public static Month November = new(1, "November");
        public static Month December = new(1, "December");
        public Month(int value, string name) : base(value, name)
        {
        }
    }
}
