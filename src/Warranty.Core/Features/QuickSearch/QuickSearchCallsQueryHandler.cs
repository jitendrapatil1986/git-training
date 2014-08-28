namespace Warranty.Core.Features.QuickSearch
{
    using System.Collections.Generic;
    using NPoco;
    using Security;

    public class QuickSearchCallsQueryHandler : IQueryHandler<QuickSearchCallsQuery, SearchResults>
    {
        private readonly IDatabase _database;
        private readonly IUserSession _userSession;

        public QuickSearchCallsQueryHandler(IDatabase database, IUserSession userSession)
        {
            _database = database;
            _userSession = userSession;
        }

        public SearchResults Handle(QuickSearchCallsQuery request)
        {
            var currentuser = _userSession.GetCurrentUser();

            //var jobs = _repository.Query<Job>()
            //    .Where(job => (currentuser.Markets.Contains(job.CostCenter.Market.JdeCode)) && job.Homeowner.Name.Like(request.Query) || job.JobNumber.Like(request.Query) || job.Lot.Address.StreetAddress1.Like(request.Query))
            //    .Select(job => new { job.Id, job.JobNumber, job.Lot.Address, Buyer = job.Homeowner.Name, IsInactive = (job.WarrantyDate.HasValue && job.CloseStatus == JobCloseStatus.InFourMonthWindow) })
            //    .Where(x => request.IncludeInactive || !x.IsInactive);

            //var result = jobs.Take(10)
            //                 .ToArray()
            //                 .Select(job => new { job.Id, job.JobNumber, Address = job.Address.ToString(), job.Buyer });

//            var data = result.ToDictionary(x => Convert.ToString(x.Id), x => x.ToJson());
            //          return new SearchResults(data) { TotalMatches = jobs.Count(), Query = request.Query };
            return new SearchResults(new Dictionary<string, string>());
        }
    }
}