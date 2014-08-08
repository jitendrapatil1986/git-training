namespace Warranty.UI.Core.Helpers
{
    public static class Css
    {
        public static string ServiceCallProgressBar(int numberOfDaysRemaining)
        {
            switch (numberOfDaysRemaining)
            {
                case 0:
                case 1:
                case 2:
                    return "progress-bar-danger";
                case 3:
                case 4:
                    return "progress-bar-warning";
                case 5:
                case 6:
                case 7:
                    return "progress-bar-success";

            }

            return "";
        }
    }
}