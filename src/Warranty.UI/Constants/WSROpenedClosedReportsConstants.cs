using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Warranty.UI.Constants
{
    public class WSROpenedClosedReportsConstants
    {
        public static readonly string StartingNumberOfCalls = "Represents all of the Service Calls not closed (this includes calls that have a status of Open, In Progress, and Pending Approval) that were assigned to the WSR at the starting date of report.";
        public static readonly string NumberOfOpenedCalls = "Represents the number of calls that have been opened within the specified date range.";
        public static readonly string NumberOfClosedCalls = "Represents the total number of service calls closed within the specified date range.";
        public static readonly string EndingNumberOfCalls = "Represents the snapshot of all Service Calls not closed (this includes calls that have a status of Open, In Progress, and Pending Approval) that were assigned to the WSR at the ending date of report.";

    }
}