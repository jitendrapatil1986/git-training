namespace Warranty.Core.DataAccess.Mappings.Lookups
{
    using Entities.Lookups;
    using NPoco.FluentMappings;

    public abstract class LookupMap<T> : Map<T> where T : LookupEntity
    {
        protected LookupMap(string tableName, string idColumnName, string displayNameColumnName)
        {
            TableName(tableName)
                .PrimaryKey(x => x.Id)
                .Columns(x =>
                             {
                                 x.Column(col => col.Id).WithName(idColumnName);
                                 x.Column(col => col.DisplayName).WithName(displayNameColumnName);
                             });
        }
    }
}