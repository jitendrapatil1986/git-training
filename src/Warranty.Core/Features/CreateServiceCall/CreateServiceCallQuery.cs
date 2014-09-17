using System;

namespace Warranty.Core.Features.CreateServiceCall
{
    public class CreateServiceCallQuery : IQuery<CreateServiceCallModel>
    {
        public Guid JobId { get; set; }
    }
}
