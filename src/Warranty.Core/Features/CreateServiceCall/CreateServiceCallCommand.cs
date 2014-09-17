namespace Warranty.Core.Features.CreateServiceCall
{
    using System;
    using System.Collections.Generic;
    using Entities;

    public class CreateServiceCallCommand : ICommand<Guid>
    {
        public Guid JobId { get; set; }
        public IEnumerable<ServiceCallLineItem> ServiceCallLineItems { get; set; }
    }
}
