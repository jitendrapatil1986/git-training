namespace Warranty.Core.DataAccess.Mappings
{
    using Entities;
    using NHibernate.Mapping.ByCode;

    public class WarrantyCallCommentMapping : AuditableEntityMapping<WarrantyCallComment>
    {
        public WarrantyCallCommentMapping()
        {
            Table("WarrantyCallComments");

            Id(x => x.WarrantyCallCommentId, map => map.Generator(Generators.GuidComb));
            Property(x => x.WarrantyCallId);
            Property(x => x.Comment, map => map.Column("WarrantyCallComment"));
        }
    }
}