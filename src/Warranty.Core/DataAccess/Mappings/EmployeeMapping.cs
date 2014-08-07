namespace Warranty.Core.DataAccess.Mappings
{
    using Entities;
    using NHibernate.Mapping.ByCode;

    public class EmployeeMapping : AuditableEntityMapping<Employee>
    {
        public EmployeeMapping()
        {
            Table("Employees");

            Id(x => x.EmployeeId, map => map.Generator(new GuidCombGeneratorDef()));
            Property(x => x.Number, map => map.Column("EmployeeNumber"));
            Property(x => x.Name, map => map.Column("EmployeeName"));
        }
    }
}