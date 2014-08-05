using NHibernate.Mapping.ByCode.Conformist;
using Warranty.Core.Entities;

namespace Warranty.Core.DataAccess.Mappings
{
    public class WarrantyCallLineItemMapping : AuditableEntityMapping<WarrantyCallLineItem>
    {
        public WarrantyCallLineItemMapping()
        {
            Table("WarrantyCallLineItems");

            Id(x => x.WarrantyCallLineItemId);
            Property(x => x.WarrantyCallId);
            Property(x => x.LineNumber);
            Property(x => x.ProblemCode);
            Property(x => x.ProblemDescription);
            Property(x => x.ClassificationNote);
            Property(x => x.LineItemRoot);
            Property(x => x.Completed);
        }
    }
}