using System;

namespace Warranty.Core.Features.Vendor
{
    public class UpdateVendorDataCommand : ICommand
    {
        public UpdateVendorDataCommand(Guid jobId)
        {
            JobId = jobId;
        }

        public Guid JobId { get; set; }
    }
}