using Warranty.Core.Enumerations;

namespace Warranty.UI.Core.Helpers
{
    using Warranty.Core.Configurations;

    public static class Css
    {
        public static string ServiceCallProgressBar(int numberOfDaysRemaining)
        {
            switch (numberOfDaysRemaining)
            {
                case 3:
                case 4:
                    return "progress-bar-warning";
                case 5:
                case 6:
                case 7:
                    return "progress-bar-success";
                default:
                    return "progress-bar-danger";
            }
        }

        public static string WarrantySpent(decimal spentAmount)
        {
            if (spentAmount <= WarrantyConstants.WarrantySpentGoal)
                return "amount-spent-green";

            if (spentAmount > WarrantyConstants.WarrantySpentGoal)
                return "amount-spent-red";

            return "";
        }

        public static string ToDoClass(ToDoType toDoType)
        {
            var toDoDisplayNameWithSingleWhiteSpace =
                System.Text.RegularExpressions.Regex.Replace(toDoType.DisplayName, @"\s+", " ").ToLower().Trim();

            return string.Format("{0}{1}", "todo-", toDoDisplayNameWithSingleWhiteSpace.Replace(" ", "-"));
        }
    }
}
