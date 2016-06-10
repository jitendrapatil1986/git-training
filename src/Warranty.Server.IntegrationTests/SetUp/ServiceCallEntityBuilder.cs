using System;
using NPoco;
using Warranty.Core.Entities;
using Warranty.Core.Enumerations;

namespace Warranty.Server.IntegrationTests.SetUp
{
    public class ServiceCallEntityBuilder : EntityBuilder<ServiceCall>
    {
        public ServiceCallEntityBuilder(IDatabase database) : base(database)
        {
        }

        public override ServiceCall GetSaved(Action<ServiceCall> action)
        {
            var r = new Random();
            var serviceCallNum = r.Next(12345678, 88889999);

            var entity = new ServiceCall
            {
                ServiceCallNumber = serviceCallNum,
                ServiceCallStatus = ServiceCallStatus.Open,
                CompletionDate = null,
                HomeownerVerificationType = HomeownerVerificationType.NotVerified,
                ServiceCallType = RequestType.WarrantyRequest.DisplayName,
            };

            return entity;  // Returning the object, instead of Saved(entity, action), since the test needs to set the CreatedDate property, 
                            // which would be auto-assigned since ServiceCalls implements IAuditableEntity.
        }
    }
}