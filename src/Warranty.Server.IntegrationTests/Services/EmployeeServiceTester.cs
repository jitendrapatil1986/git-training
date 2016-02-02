using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Should;
using StructureMap;
using Warranty.Core.Entities;
using Warranty.Core.Services;
using Warranty.Server.IntegrationTests.Extensions.IDatabase;
using Warranty.Server.IntegrationTests.SetUp;

namespace Warranty.Server.IntegrationTests.Services
{
    [TestFixture]
    public class EmployeeServiceTester : ServicesTestBase
    {
        private IEmployeeService _employeeService;

        public EmployeeServiceTester()
        {
            _employeeService = ObjectFactory.GetInstance<IEmployeeService>();
        }

        [Test]
        public void GetEmployeeByNumberTest()
        {
            var employee = Get<Employee>();
            var queriedEmployee = _employeeService.GetEmployeeByNumber(employee.Number);

            queriedEmployee.EmployeeId.ShouldEqual(employee.EmployeeId);
        }
    }
}
