using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPoco;
using Warranty.Core.Entities;

namespace Warranty.IntegrationTests.MediatorMessagingTests.EntityBuilders
{
    public class ServiceCallNoteEntityBuilder: EntityBuilder<ServiceCallNote>
    {
        public ServiceCallNoteEntityBuilder(IDatabase database) : base(database)
        {
        }

        public override ServiceCallNote GetSaved(Action<ServiceCallNote> action)
        {
            var entity = new ServiceCallNote();
            return Saved(entity, action);
        }
    }
}
