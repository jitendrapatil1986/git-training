namespace Warranty.Core.Services
{
    using System;
    using Configurations;

    public static class ServiceCallCalculator
    {
        public static int CalculatePercentComplete(int numberOfDaysRemaining)
        {
            if (numberOfDaysRemaining <= 0)
                return 100;

            if (numberOfDaysRemaining > WarrantyConstants.NumberOfDaysAllowedToCloseServiceCall)
                return 0;

            var complete = (decimal)(WarrantyConstants.NumberOfDaysAllowedToCloseServiceCall - numberOfDaysRemaining) / WarrantyConstants.NumberOfDaysAllowedToCloseServiceCall * 100;
            return Convert.ToInt16(complete);
        }

        public static int CalculateNumberOfDaysRemaining(DateTime startDate)
        {
            if (startDate > DateTime.Today)
                return WarrantyConstants.NumberOfDaysAllowedToCloseServiceCall;

            var numberOfDaysLeft = WarrantyConstants.NumberOfDaysAllowedToCloseServiceCall - (int) (DateTime.Today - startDate.Date).TotalDays;
            return numberOfDaysLeft;
        }
    }
}