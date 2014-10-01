namespace Warranty.Core.Features.ServiceCallStats
{
    using Enumerations;

    public class ServiceCallStatsQuery : IQuery<ServiceCallStatsModel>
    {
        public StatView ViewId { get; set; }
    }
}
