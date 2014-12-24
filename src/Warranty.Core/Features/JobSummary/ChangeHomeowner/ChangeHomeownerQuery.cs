namespace Warranty.Core.Features.JobSummary.ChangeHomeowner
{
    using System;

    public class ChangeHomeownerQuery : IQuery<ChangeHomeownerModel>
    {
        public Guid JobId { get; set; }
        public ChangeHomeownerModel ChangeHomeowner { get; set; }

        public ChangeHomeownerQuery()
        {
            ChangeHomeowner = new ChangeHomeownerModel();
        }
    }
}