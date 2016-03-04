using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPoco;
using Warranty.Core.Entities;

namespace Warranty.IntegrationTests.MediatorMessagingTests.EntityBuilders
{
    public class PurchaseOrderEntityBuilder : EntityBuilder<PurchaseOrder>
    {
        public PurchaseOrderEntityBuilder(IDatabase database) : base(database)
        {
        }

        public override PurchaseOrder GetSaved(Action<PurchaseOrder> action)
        {
            var entity = new PurchaseOrder
            {
                CreatedBy = "test"
            };

            return Saved(entity, action);
        }
    }
}
