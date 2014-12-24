namespace Warranty.Core.Services
{
    using System;
    using Entities;
    using Enumerations;
    using InnerMessages;
    using NPoco;
    using NServiceBus;

    public class ServiceCallCreateService : IServiceCallCreateService
    {
        private readonly IDatabase _database;
        private readonly IBus _bus;
        private readonly ICommunityEmployeeService _communityEmployeeService;

        public ServiceCallCreateService(IDatabase database, IBus bus, ICommunityEmployeeService communityEmployeeService)
        {
            _database = database;
            _bus = bus;
            _communityEmployeeService = communityEmployeeService;
        }

        public Guid Create(Guid jobId, RequestType requestType, ServiceCallStatus serviceCallStatus)
        {
            using (_database)
            {
                const string sql = @"SELECT TOP 1 ServiceCallNumber FROM ServiceCalls ORDER BY ServiceCallNumber DESC";
                var lastCallNumber = _database.ExecuteScalar<int>(sql);
                var newCallNumber = lastCallNumber + 1;

                var employeeId = _communityEmployeeService.GetEmployeeIdForWsc(jobId);

                var serviceCall = new ServiceCall
                {
                    ServiceCallNumber = newCallNumber,
                    ServiceCallStatus = serviceCallStatus,
                    ServiceCallType = requestType.DisplayName,
                    JobId = jobId,
                    WarrantyRepresentativeEmployeeId = employeeId,
                    HomeownerVerificationType = HomeownerVerificationType.NotVerified,
                };

                _database.Insert(serviceCall);
                _bus.Send<NotifyServiceCallCreated>(x => { x.ServiceCallId = serviceCall.ServiceCallId; });

                return serviceCall.ServiceCallId;
            }
        }
    }
}