using System;
using System.Linq;
using Moq;
using NUnit.Framework;
using Warranty.Core.Calculator;
using Warranty.Core.Services;

namespace Warranty.Tests.Core.Calculator
{
    [TestFixture]
    public class WarrantyCalculatorTests
    {
        private Mock<IEmployeeService> _employeeServiceMoq;

        [SetUp]
        public void Setup()
        {
            _employeeServiceMoq = new Mock<IEmployeeService>();
            _employeeServiceMoq.Setup((a) => a.GetEmployeeMarkets()).Returns(null as string);
        }

        [TearDown]
        public void TearDown()
        {
            //executed after every test
            //dispose objects set in setup
            //if modifying db, rollback
        }

        [Test]
        public void When_GetMonthRangeEnddate_isBeforeStartdate_thenThrowException()
        {
            var wc = new WarrantyCalculator(null, null, _employeeServiceMoq.Object);
            Assert.Throws<ArgumentException>(() => wc.GetMonthRange(DateTime.Now.AddDays(1), DateTime.Now));
        }

        [Test]
        public void When_GetMonthRangeEnddate_isSameAsStartdate_thenReturnSingleMonthYearModel()
        {
            var wc = new WarrantyCalculator(null, null, _employeeServiceMoq.Object);
            var now = DateTime.Now;
            var localResults = wc.GetMonthRange(now, now);
            Assert.AreEqual(1, localResults.Count());
            Assert.AreEqual(now.Month, localResults.First().MonthNumber);
            Assert.AreEqual(now.Year, localResults.First().YearNumber);
        }
    }
}
