namespace Warranty.Core.Features.Report.WSROpenedClosedCalls
{
    using System;
    using System.Collections.Generic;
    using Security;
    using Enumerations;
    using NPoco;
    using Common.Extensions;

    public class WSROpenedClosedCallsQueryHandler : IQueryHandler<WSROpenedClosedCallsQuery, WSROpenedClosedCallsModel>
    {
        private readonly IDatabase _database;
        private readonly IUserSession _userSession;

        public WSROpenedClosedCallsQueryHandler(IDatabase database, IUserSession userSession)
        {
            _database = database;
            _userSession = userSession;
        }

        public WSROpenedClosedCallsModel Handle(WSROpenedClosedCallsQuery query)
        {
            var user = _userSession.GetCurrentUser();

            if (!query.queryModel.StartDate.HasValue || !query.queryModel.EndDate.HasValue)
            {
                return new WSROpenedClosedCallsModel();
            }

            var model = new WSROpenedClosedCallsModel
                {
                    WSRSummaryLines = GetWSROpenedClosedCallSummary(user, query.queryModel.StartDate, query.queryModel.EndDate),
                };

            model.WSRSummaryLines.ForEach(x => x.EmployeeName = x.EmployeeName.ToTitleCase());
            return model;
        }

        private IEnumerable<WSROpenedClosedCallsModel.WSRSummaryLine> GetWSROpenedClosedCallSummary(IUser user, DateTime? startdate, DateTime? endDate)
        {
            using (_database)
            {
                const string sql = @"SELECT EmployeeNumber, EmployeeName, NumberCallsBeforeTimeframe, NumberCallsOpenedDuringTimeframe, NumberCallsClosedDuringTimeframe,
                                        NumberCallsBeforeTimeframe + NumberCallsOpenedDuringTimeframe + NumberCallsClosedDuringTimeframe AS NumberCallsAfterTimeframe
                                    FROM
                                    (
                                    SELECT e.EmployeeNumber, e.EmployeeName,
                                        SUM(CASE WHEN (sc.CreatedDate < @0) AND (sc.ServiceCallStatusId <> @2) THEN 1 ELSE 0 END) as NumberCallsBeforeTimeframe,
                                        SUM(CASE WHEN (sc.CreatedDate BETWEEN @0 AND @1) AND (sc.ServiceCallStatusId = @3) THEN 1 ELSE 0 END) as NumberCallsOpenedDuringTimeframe,
                                        SUM(CASE WHEN (sc.CreatedDate BETWEEN @0 AND @1) AND (sc.ServiceCallStatusId = @4) THEN 1 ELSE 0 END) as NumberCallsClosedDuringTimeframe
                                    FROM ServiceCalls sc
                                    INNER JOIN Jobs j
                                    ON sc.JobId = j.JobId
                                    INNER JOIN Communities cm
                                    ON j.CommunityId = cm.CommunityId
                                    INNER JOIN Cities ci
                                    ON cm.CityId = ci.CityId
                                    INNEr JOIN Employees e
                                    ON sc.WarrantyRepresentativeEmployeeId = e.EmployeeId
                                    WHERE ci.CityCode IN ({0})
                                    AND sc.CreatedDate <= @1
                                    GROUP BY e.EmployeeNumber, e.EmployeeName
                                    ) a
                                    ORDER BY EmployeeName";

                var results = _database.Fetch<WSROpenedClosedCallsModel.WSRSummaryLine>(string.Format(sql, user.Markets.CommaSeparateWrapWithSingleQuote()), startdate, endDate,
                                ServiceCallStatus.Requested.Value, ServiceCallStatus.Open.Value, ServiceCallStatus.Complete.Value);

                return results;
            }
        }
    }
}