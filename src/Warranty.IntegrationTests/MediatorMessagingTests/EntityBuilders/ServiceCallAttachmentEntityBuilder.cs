using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPoco;
using Warranty.Core.Entities;

namespace Warranty.IntegrationTests.MediatorMessagingTests.EntityBuilders
{
    public class ServiceCallAttachmentEntityBuilder : EntityBuilder<ServiceCallAttachment>
    {
        public ServiceCallAttachmentEntityBuilder(IDatabase database) : base(database)
        {
        }

        public override ServiceCallAttachment GetSaved(Action<ServiceCallAttachment> action)
        {
            var entity = new ServiceCallAttachment();
            return Saved(entity, action);
        }
    }
}
