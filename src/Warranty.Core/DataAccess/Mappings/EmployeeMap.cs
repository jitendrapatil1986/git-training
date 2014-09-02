namespace Warranty.Core.DataAccess.Mappings
{
    using Entities;

    public class EmployeeMap : AuditableEntityMap<Employee>
    {
        public EmployeeMap()
        {
            TableName("Employees")
                .PrimaryKey(x => x.EmployeeId, false)
                .Columns(x =>
                             {
                                 x.Column(col => col.Number).WithName("EmployeeNumber");
                                 x.Column(col => col.Name).WithName("EmployeeName");
                                 x.Column(col => col.JdeIdentifier);
                                 x.Column(col => col.Fax).Ignore();
                             });
        }
    }
}
