namespace Warranty.Core.Services
{
    using System;
    using Enumerations;

    public interface IServiceCallCreateService
    {
        Guid CreateServiceCallForTwelveMonthAnniversary(Guid jobId, RequestType requestType, ServiceCallStatus serviceCallStatus);
    }
}