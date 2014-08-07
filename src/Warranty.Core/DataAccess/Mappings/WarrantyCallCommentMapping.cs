namespace Warranty.Core.DataAccess.Mappings
{
    using Entities;

    public class WarrantyCallCommentMapping : AuditableEntityMapping<WarrantyCallComment>
    {
        public WarrantyCallCommentMapping()
        {
            Table("WarrantyCallComments");

            Id(x => x.WarrantyCallCommentId);
            Property(x => x.WarrantyCallId);
            Property(x => x.Comment, map => map.Column("WarrantyCallComment"));
        }
    }
}