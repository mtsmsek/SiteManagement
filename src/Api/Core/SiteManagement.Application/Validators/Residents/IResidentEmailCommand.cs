using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.Application.Validators.Residents;

public interface IResidentEmailCommand
{
    public string Email { get; set; }
}
