using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.Application.Features.Commands.Payments.CreatePayment;

public class CreatePaymentResponse
{
    public string BillType { get; set; }
    public string Period { get; set; }
    public double Fee { get; set; }
}
