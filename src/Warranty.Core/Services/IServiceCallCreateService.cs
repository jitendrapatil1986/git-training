namespace Warranty.Core.Services
{
    using System;
    using Enumerations;

    public interface IServiceCallCreateService
    {
        Guid Create(Guid jobId, RequestType requestType, ServiceCallStatus serviceCallStatus);
    }
}