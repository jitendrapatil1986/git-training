using Warranty.Core.Entities;

namespace Warranty.Core.DataAccess.Mappings
{
    public class EmployeeMapping : AuditableEntityMapping<Employee>
    {
        public EmployeeMapping()
        {
            Table("Employees");

            Id(x => x.EmployeeId);
            Property(x => x.Number, map => map.Column("EmployeeNumber"));
            Property(x => x.Name, map => map.Column("EmployeeName"));
        }
    }
}