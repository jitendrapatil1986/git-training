namespace Warranty.Core.DataAccess.Mappings
{
    using Entities;

    public class ServiceCallMap : AuditableEntityMap<ServiceCall>
    {
        public ServiceCallMap()
        {
            TableName("ServiceCalls")
                .PrimaryKey("ServiceCallId", false)
                .Columns(x =>
                {
                    x.Column(y => y.ServiceCallId);
                    x.Column(y => y.ServiceCallNumber);
                    x.Column(y => y.ServiceCallType);
                    x.Column(y => y.ServiceCallStatus).WithName("ServiceCallStatusId");
                    x.Column(y => y.JobId);
                    x.Column(y => y.Contact);
                    x.Column(y => y.WarrantyRepresentativeEmployeeId);
                    x.Column(y => y.CompletionDate);
                    x.Column(y => y.WorkSummary);
                    x.Column(y => y.HomeOwnerSignature);
                    x.Column(y => y.IsSpecialProject).WithName("SpecialProject");
                    x.Column(y => y.IsEscalated).WithName("Escalated");
                });
        }
    }
}