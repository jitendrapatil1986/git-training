using System;
using NPoco;
using Warranty.Core.Entities;

namespace Warranty.Server.IntegrationTests.SetUp
{
    public class EmployeeEntityBuilder : EntityBuilder<Employee>
    {
        public EmployeeEntityBuilder(IDatabase database)
            : base(database)
        {
        }

        public override Employee GetSaved(Action<Employee> action)
        {
            var entity = new Employee
                             {
                                 Name = "Test Employee",
                                 Number = "123456",
                                 JdeIdentifier = "99123456",
                             };

            return Saved(entity, action);
        }
    }
}