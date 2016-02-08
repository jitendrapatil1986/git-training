using NUnit.Framework;
using Should;
using StructureMap;
using Warranty.Core.Entities;
using Warranty.Core.Services;

namespace Warranty.Server.IntegrationTests.Services
{
    [TestFixture]
    public class EmployeeServiceTester : ServiceTesterBase
    {
        private IEmployeeService _employeeService;

        public EmployeeServiceTester()
        {
            _employeeService = ObjectFactory.GetInstance<IEmployeeService>();
        }

        [TestCase("0", "0", "00000")]
        [TestCase("1", "000001", "321")]
        [TestCase("100", "00100", "32100")]
        [TestCase("00y","00y","00y0")]
        [TestCase("1", "  1", "   101")]
        public void QueryEmployeeNumber(string employeeNumberToSearch, string employeeNumberFromSql, string fakeEmployeeNumber)
        {
            var employee = Get<Employee>(e =>
            {
                e.Number = employeeNumberFromSql;
            });
            var fakeEmployee = Get<Employee>(e =>
            {
                e.Number = fakeEmployeeNumber;
            });
            var employeeFromSql = _employeeService.GetEmployeeByNumber(employeeNumberToSearch);
            employeeFromSql.EmployeeId.ShouldEqual(employee.EmployeeId);
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
