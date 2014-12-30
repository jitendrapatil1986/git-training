namespace Warranty.IntegrationTests.MediatorMessagingTests.EntityBuilders
{
    using System;
    using System.Globalization;
    using NPoco;
    using Warranty.Core.Entities;

    public class EmployeeEntityBuilder : EntityBuilder<Employee>
    {
        public EmployeeEntityBuilder(IDatabase database)
            : base(database)
        {
        }

        public override Employee GetSaved(Action<Employee> action)
        {
            var r = new Random();
            var empNum = r.Next(100000, 999999).ToString(CultureInfo.InvariantCulture);

            var entity = new Employee
            {
                Name = "Test Employee",
                Number = empNum,
                EmployeeId = Guid.NewGuid(),
            };

            return Saved(entity, action);
        }
    }
}