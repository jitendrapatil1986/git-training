using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NServiceBus;
using TIPS.Events.JobEvents;

namespace Warranty.Server.Handlers.Jobs
{
    public class BuyerTransferApprovedHandler : IHandleMessages<BuyerTransferApproved>
    {
        public BuyerTransferApprovedHandler()
        {
            
        }
    }
}
