using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPoco;
using Warranty.Core.Entities;
using Warranty.Core.Enumerations;

namespace Warranty.IntegrationTests.MediatorMessagingTests.EntityBuilders
{
    public class ServiceCallLineItemEntityBuilder : EntityBuilder<ServiceCallLineItem>
    {
        public ServiceCallLineItemEntityBuilder(IDatabase database) : base(database)
        {
        }

        public override ServiceCallLineItem GetSaved(Action<ServiceCallLineItem> action)
        {
            var entity = new ServiceCallLineItem
            {
                ServiceCallLineItemStatus = ServiceCallLineItemStatus.Open
            };

            return Saved(entity, action);
        }
    }
}
