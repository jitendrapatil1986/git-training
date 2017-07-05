namespace Warranty.Core.Features.BuiltTheWeekleyWay
{
    using Common.Security.Session;
    using NPoco;
    using Services;
    using System.Linq;
    using System.Text.RegularExpressions;

    public class BuiltTheWeekleyWayQueryHandler : IQueryHandler<BuiltTheWeekleyWayQuery, BuiltTheWeekleyWayModel>
    {        
        private readonly IUserSession _userSession;
        private readonly IBuiltTheWeekleyWayService _builtTheWeekleyWayService;

        public BuiltTheWeekleyWayQueryHandler(IUserSession userSession, IBuiltTheWeekleyWayService builtTheWeekleyWayService)
        {
            _userSession = userSession;
            _builtTheWeekleyWayService = builtTheWeekleyWayService;
        }

        public BuiltTheWeekleyWayModel Handle(BuiltTheWeekleyWayQuery query)
        {
            var user = _userSession.GetCurrentUser();
            var marketCode = user.Markets.First();

            var files = _builtTheWeekleyWayService.GetFiles(marketCode);
            Regex regex = new Regex(@"^(?<beforeDecimal>\d*)\.(?<afterDecimal>\d*)");

            // files sequence can be 0.1, 1.10, 1.1, 1.11 , 2.10, 2.1 etc.. so this code will sort the file order and display proper file name.
            return new BuiltTheWeekleyWayModel
            {
                MarketName = marketCode,                
                Contents = files.OrderBy(x => regex.IsMatch(x.Name) ? int.Parse(regex.Match(x.Name).Groups["beforeDecimal"].Value) : int.MaxValue)
                                .ThenBy(x => regex.IsMatch(x.Name) ? int.Parse(regex.Match(x.Name).Groups["afterDecimal"].Value) : int.MaxValue)
                                .ThenBy(x => x.Name)
                                .Select(y => new BuiltTheWeekleyWayModel.ContentRow
                                {
                                    DisplayName = _builtTheWeekleyWayService.GetDisplayName(y.Name),
                                    PathToFile = y.FullName,
                                    FileName = y.Name,
                                    MarketCode = marketCode
                                }).ToList()
            };
        }
    }
}
