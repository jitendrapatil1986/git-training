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
            var testString = "test";

            var entity = new ServiceCallLineItem
            {
            };

            return Saved(entity, action);
        }
    }
}
