using SiteManagement.Domain.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.Domain.Enumarations.Security
{
    public class AuthenticatorType : Enumaration<AuthenticatorType>
    {
        public static AuthenticatorType None = new(1, "None");
        public static AuthenticatorType Email = new(2, "Email");
        public static AuthenticatorType Otp = new(3, "Otp");
        public AuthenticatorType(int value, string name) : base(value, name)
        {
        }
    }
}
