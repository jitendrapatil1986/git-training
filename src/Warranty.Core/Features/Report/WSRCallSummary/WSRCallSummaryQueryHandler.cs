namespace Warranty.Core.Features.Report.WSRCallSummary
{
    using System.Collections.Generic;
    using Enumerations;
    using NPoco;
    using Security;

    public class WSRCallSummaryQueryHandler : IQueryHandler<WSRCallSummaryQuery, WSRCallSummaryModel>
    {
        private readonly IDatabase _database;
        private readonly IUserSession _userSession;

        public WSRCallSummaryQueryHandler(IDatabase database, IUserSession userSession)
        {
            _database = database;
            _userSession = userSession;
        }

        public WSRCallSummaryModel Handle(WSRCallSummaryQuery query)
        {
            var user = _userSession.GetCurrentUser();

            using (_database)
            {
                var model = new WSRCallSummaryModel
                    {
                        EmployeeName = user.UserName,
                        EmployeeNumber = user.EmployeeNumber,
                        ServiceCalls = GetWSRServiceCalls(user.EmployeeNumber),
                    };

                return model;
            }
        }

        private IEnumerable<WSRCallSummaryModel.ServiceCall> GetWSRServiceCalls(string employeeNumber)
        {
            using (_database)
            {
                const string sql = @"SELECT 
                                        sc.ServiceCallId as ServiceCallId
                                        , Servicecallnumber as CallNumber
                                        , j.AddressLine as [Address]
                                        , j.City 
                                        , j.StateCode
                                        , j.PostalCode
                                        , ho.HomeOwnerName
                                        , ho.HomePhone
                                        , ho.OtherPhone
                                        , cm.CommunityName
                                        , li.LineNumber
                                        , li.ProblemCode
	                                    , li.ProblemDescription
                                    FROM ServiceCalls sc
                                    INNER JOIN Jobs j
                                    ON sc.JobId = j.JobId
                                    INNER JOIN HomeOwners ho
                                    ON j.CurrentHomeOwnerId = ho.HomeOwnerId
                                    LEFT JOIN ServiceCallLineItems li
                                    ON sc.ServiceCallId = li.ServiceCallId
                                    LEFT JOIN Employees e
                                    ON sc.WarrantyRepresentativeEmployeeId = e.EmployeeId
                                    INNER JOIN Communities cm
                                    ON j.CommunityId = cm.CommunityId
                                    WHERE e.EmployeeNumber = @0
                                    AND sc.ServiceCallStatusId = 2
                                    ORDER BY cm.CommunityName, ho.HomeOwnerName, j.AddressLine";

                var serviceCalls = _database.FetchOneToMany<WSRCallSummaryModel.ServiceCall, WSRCallSummaryModel.ServiceCallLine>(x => x.ServiceCallId, sql, employeeNumber);
                
                foreach (var serviceCall in serviceCalls)
                {
                    const string notesSql = @"SELECT ServiceCallNote AS Note FROM ServiceCallNotes WHERE ServiceCallId = @0 AND (ServiceCallNote <> '' OR ServiceCallNote != null)";

                    serviceCall.ServiceCallNotes = _database.Fetch<WSRCallSummaryModel.ServiceCallNote>(notesSql, serviceCall.ServiceCallId);
                }

                return serviceCalls;
            }
        }
    }
}