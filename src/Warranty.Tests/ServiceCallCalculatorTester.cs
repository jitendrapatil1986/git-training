namespace Warranty.Tests
{
    using System;
    using NUnit.Framework;
    using Should;
    using Warranty.Core.Configurations;
    using Warranty.Core.Services;

    [TestFixture]
    public class ServiceCallCalculatorTester
    {
        [Test]
        public void NumberOfDaysRemaining_should_return_0_when_start_date_is_over_seven_days_ago()
        {
            var result = ServiceCallCalculator.CalculateNumberOfDaysRemaining(DateTime.Today.AddDays(-10));
            result.ShouldEqual(0);
        }
        
        [Test]
        public void NumberOfDaysRemaining_should_return_1_when_start_date_is_six_days_ago()
        {
            var result = ServiceCallCalculator.CalculateNumberOfDaysRemaining(DateTime.Today.AddDays(-6));
            result.ShouldEqual(1);
        }

        [Test]
        public void NumberOfDaysRemaining_should_return_3_when_start_date_is_four_days_ago()
        {
            var result = ServiceCallCalculator.CalculateNumberOfDaysRemaining(DateTime.Today.AddDays(-4));
            result.ShouldEqual(3);
        }

        [Test]
        public void NumberOfDaysRemaining_should_return_7_when_start_date_is_today()
        {
            var result = ServiceCallCalculator.CalculateNumberOfDaysRemaining(DateTime.Today);
            result.ShouldEqual(WarrantyConstants.NumberOfDaysAllowedToCloseServiceCall);
        }

        [Test]
        public void NumberOfDaysRemaining_should_return_7_when_start_date_is_in_the_future()
        {
            var result = ServiceCallCalculator.CalculateNumberOfDaysRemaining(DateTime.Today.AddDays(5));
            result.ShouldEqual(7);
        }

        [Test]
        public void CalculatePercentComplete_should_return_100_when_0_days_remaining()
        {
            var result = ServiceCallCalculator.CalculatePercentComplete(0);
            result.ShouldEqual(100);
        }

        [Test]
        public void CalculatePercentComplete_should_return_0_when_number_days_remaining_greater_than_allowed()
        {
            var result = ServiceCallCalculator.CalculatePercentComplete(9);
            result.ShouldEqual(0);
        }

        [Test]
        public void CalculatePercentComplete_should_return_43_when_4_days_remain_of_7()
        {
            var result = ServiceCallCalculator.CalculatePercentComplete(4);
            result.ShouldEqual(43);
        }

        [Test]
        public void CalculatePercentComplete_should_return_14_when_6_days_remain_of_7()
        {
            var result = ServiceCallCalculator.CalculatePercentComplete(6);
            result.ShouldEqual(14);
        }
    }
}
/*
            if (numberOfDaysRemaining == 0)
                return 100;

            var complete = (WarrantyConstants.NumberOfDaysAllowedToCloseServiceCall - numberOfDaysRemaining) / WarrantyConstants.NumberOfDaysAllowedToCloseServiceCall * 100;
            return Convert.ToInt16(complete);

*/