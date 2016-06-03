using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Warranty.Core.Enumerations;
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
        public string Action { get; set; }

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

        public TimeSpan OldestOpenTask
        {
            get
            {
                if(OpenTasks == null || !OpenTasks.Any())
                    return TimeSpan.Zero;
                return OpenTasks.Max(t => t.DaysOutstanding);
            }
        }

        public string Anchor
        {
            get
            {
                return new Regex("[^a-zA-Z0-9 -]").Replace(EmployeeName, "").Trim().Replace(" ", null);
            }
        }
    }

    public class OpenTask
    {
        public string Task { get; set; }

        public string Description { get; set; }

        public DateTime? CreatedDate { get; set; }

        public Guid? ReferenceId { get; set; }

        public TimeSpan DaysOutstanding
        {
            get
            {
                if(!CreatedDate.HasValue)
                    return TimeSpan.Zero;

                return DateTime.UtcNow.Subtract(CreatedDate.Value);
            }
        }

        public TaskType TaskType { get; set; }

        public string JobNumber { get; set; }
    }
}