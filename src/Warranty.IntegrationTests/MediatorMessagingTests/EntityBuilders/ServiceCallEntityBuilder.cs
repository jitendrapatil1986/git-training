namespace Warranty.IntegrationTests.MediatorMessagingTests.EntityBuilders
{
    using System;
    using Warranty.Core.Entities;
    using Warranty.Core.Enumerations;
    using NPoco;

    public class ServiceCallEntityBuilder : EntityBuilder<ServiceCall>
    {
        public ServiceCallEntityBuilder(IDatabase database)
            : base(database)
        {
        }

        public override ServiceCall GetSaved(Action<ServiceCall> action)
        {
            var job = GetSaved<Job>();
            var employee = GetSaved<Employee>();
            var r = new Random();
            var serviceCallNum = r.Next(12345678, 88889999);

            var entity = new ServiceCall()
                {
                    CreatedBy = "test",
                    CreatedDate = DateTime.UtcNow,
                    ServiceCallNumber = serviceCallNum,
                    WarrantyRepresentativeEmployeeId = employee.EmployeeId,
                    JobId = job.JobId,
                    ServiceCallStatus = ServiceCallStatus.Requested,
                    HomeownerVerificationType = HomeownerVerificationType.NotVerified,
                    ServiceCallType = RequestType.WarrantyRequest.DisplayName
                };

            return Saved(entity, action);
        }
    }
}