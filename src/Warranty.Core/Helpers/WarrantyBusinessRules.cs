using System;

namespace Warranty.Core.Helpers
{
    public static class WarrantyBusinessRules
    {
        public static int ServiceCallPercentComplete(int numberOfDaysRemaining)
        {
            if (numberOfDaysRemaining == 0)
                return 100;

            var complete = (7.0 - numberOfDaysRemaining) / 7.0 * 100;
            return Convert.ToInt16(complete);
        }
    }
}