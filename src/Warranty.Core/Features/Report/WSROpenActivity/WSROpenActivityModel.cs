using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Warranty.Core.Enumerations;

namespace Warranty.Core.Features.Report.WSROpenActivity
{
    public class WSROpenActivityModel
    {
        public WSROpenActivityModel()
        {
            MyDivisions = new List<SelectListItem>();
            MyTeamMembers = new List<SelectListItem>();
            MyProjects = new List<SelectListItem>();
        }

        public List<SelectListItem> MyDivisions { get; set; }
        public List<SelectListItem> MyTeamMembers { get; set; }
        public List<SelectListItem> MyProjects { get; set; }

        public Guid? DivisionId { get; set; }
        public Guid? ProjectId { get; set; }
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