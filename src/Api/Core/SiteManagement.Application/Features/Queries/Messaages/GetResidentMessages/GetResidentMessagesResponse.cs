using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.Application.Features.Queries.Messaages.GetResidentMessages
{
    public class GetResidentMessagesResponse
    {
        public string SenderName { get; set; }
        public string Message { get; set; }
        public string CreatedDate { get; set; }
        public bool IsSeen { get; set; }
    }
}
