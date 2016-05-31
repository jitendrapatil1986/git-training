namespace Warranty.UI.Constants
{
    public class SaltlineReportConstants
    {
        public static readonly string HomesUnderWarrantyTooltip = "Homes Under Warranty is the total number of homes in the communities assigned to " +
                                                                      "each WSR that have a closing date within the last 2 years as of the last day of the specified month.";
        public static readonly string NumberOfCallsTooltip = "The total open outstanding calls assigned to the WSR as of the last day of the specified month.";
        public static readonly string AverageDaysTooltip = "The average days outstanding of the open calls as of the last day of the specified month.";

        public static readonly string NumberOfSurveysTooltip = "The total number of surveys each WSR received during the specified date range.";
        public static readonly string TrulyOustandingWarrantyServiceTooltip = "The average survey score for each WSR for the surveys received during the specified date range based on the Number of Surveys column.";
        public static readonly string DefinitelyWouldRecommendTooltip = "The average survey score for each WSR for the surveys received during the specified date range based on the Number of Surveys column.";

        public static readonly string DollarsSpentTooltip = "The total $ amount charged to the Warranty Cost Codes for all jobs under warranty in the communities assigned to each WSR " +
                                                            "during the specified date range divided by the toal number of homes under warranty in the communtiies assigned to each WSR " +
                                                            "during the specified date range.";
    }
}