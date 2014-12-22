namespace Warranty.Core.DataAccess.Mappings
{
    using Entities;

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
                            x.Column(col => col.AreaCode);
                            x.Column(col => col.AreaName);
                        });
        }
    }
}