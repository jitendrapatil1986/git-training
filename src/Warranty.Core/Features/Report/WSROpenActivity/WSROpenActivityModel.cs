using System;
using System.Collections.Generic;
using System.Linq;
using Warranty.Core.Features.MyTeam;

namespace Warranty.Core.Features.Report.WSROpenActivity
{
    public class WSROpenActivityModel
    {
        public Dictionary<Guid, string> Divisions { get; set; }
        public Guid? DivisionId { get; set; }
        public Dictionary<Guid, string> Projects { get; set; }
        public Guid? ProjectId { get; set; }
        public IEnumerable<MyTeamModel> TeamMembers { get; set; }
        public Guid? TeamMemberId { get; set; }

        public IEnumerable<OpenActivity> OpenActivities { get; set; }

        public bool HasResults
        {
            get { return OpenActivities != null && OpenActivities.Any(); }
        }

        public Guid? FilterValue
        {
            get
            {
                if (TeamMemberId.HasValue)
                    return TeamMemberId;
                if (ProjectId.HasValue)
                    return ProjectId;
                if(DivisionId.HasValue)
                    return DivisionId;
                return null;
            }
        }
    }

    public class OpenActivity
    {
        public string EmployeeName { get; set; }
        public IEnumerable<ServiceCall> ServiceCalls { get; set; }
        public IEnumerable<OpenTask> OpenTasks { get; set; }
    }

    public class OpenTask
    {
        public string Task { get; set; }

        public string Description { get; set; }

        public DateTime? CreateDate { get; set; }

        public TimeSpan DaysOutstanding
        {
            get
            {
                if(!CreateDate.HasValue)
                    return TimeSpan.Zero;

                return DateTime.UtcNow.Subtract(CreateDate.Value);
            }
        }
    }
}