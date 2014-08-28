namespace Warranty.Core.DataAccess.Mappings
{
    using Entities;
    using NHibernate.Mapping.ByCode;

    public class DivisionMapping : AuditableEntityMapping<Division>
    {
        public DivisionMapping()
        {
            Table("Divisions");

            Id(x => x.DivisionId, map => map.Generator(Generators.GuidComb));
            Property(x => x.DivisionCode);
            Property(x => x.DivisionName);
        }
    }

    public class DivisionMap : AuditableEntityMap<Division>
    {
        public DivisionMap()
        {
            TableName("Divisions");
            PrimaryKey(x => x.DivisionId, false);

            Columns(x =>
                        {
                            x.Column(col => col.DivisionCode);
                            x.Column(col => col.DivisionName);
                        });
        }
    }
}