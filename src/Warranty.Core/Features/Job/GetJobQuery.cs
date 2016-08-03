namespace Warranty.Core.Features.Job
{
    public class GetJobQuery : IQuery<Entities.Job>
    {
        public GetJobQuery(string jobNumber)
        {
            JobNumber = jobNumber;
        }

        public string JobNumber { get; set; }
    }
}