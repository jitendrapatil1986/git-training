namespace Warranty.Core.DataAccess.Mappings
{
    using Entities;

    public class EmployeeMap : AuditableEntityMap<Employee>
    {
        public EmployeeMap()
        {
            TableName("Employees")
                .PrimaryKey("EmployeeId", false)
                .Columns(x =>
                {
                    x.Column(y => y.EmployeeId);
                    x.Column(y => y.Number).WithName("EmployeeNumber");
                    x.Column(y => y.Name).WithName("EmployeeName");
                    x.Column(y => y.Fax).Ignore();
                });
        }
    }
}
