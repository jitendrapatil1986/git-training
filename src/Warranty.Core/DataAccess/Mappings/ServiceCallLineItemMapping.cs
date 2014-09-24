namespace Warranty.Core.DataAccess.Mappings
{
    using Entities;
    using NHibernate.Mapping.ByCode;

    public class ServiceCallLineItemMapping : AuditableEntityMapping<ServiceCallLineItem>
    {
        public ServiceCallLineItemMapping()
        {
            Table("ServiceCallLineItems");

            Id(x => x.ServiceCallLineItemId, map => map.Generator(Generators.GuidComb));
            Property(x => x.ServiceCallId);
            Property(x => x.LineNumber);
            Property(x => x.ProblemCode);
            Property(x => x.ProblemDescription);
            Property(x => x.CauseDescription);
            Property(x => x.ClassificationNote);
            Property(x => x.LineItemRoot);
            Property(x => x.ServiceCallLineItemStatus, map => map.Column("ServiceCallLineItemStatusId"));
        }
    }
}