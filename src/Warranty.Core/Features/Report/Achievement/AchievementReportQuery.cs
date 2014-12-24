namespace Warranty.Core.Features.Report.Achievement
{
    using System;

    public class AchievementReportQuery : IQuery<AchievementReportModel>
    {
        public AchievementReportModel queryModel { get; set; }

        public AchievementReportQuery()
        {
            queryModel = new AchievementReportModel();
        }
    }
}