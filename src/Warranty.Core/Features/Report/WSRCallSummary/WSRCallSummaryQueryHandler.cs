namespace Warranty.Core.Features.Report.WSRCallSummary
{
    using System.Collections.Generic;
    using Enumerations;
    using NPoco;
    using Common.Security.Session;
    using System.Linq;
    using Common.Extensions;

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

            if (!user.IsInRole(UserRoles.WarrantyServiceRepresentative) && query.queryModel.SelectedEmployeeNumber == null)
            {
                return new WSRCallSummaryModel
                    {
                        EmployeeTiedToRepresentatives = GetEmployeesTiedToRepresentatives(user),
                    };
            }

            var employeeNumber = user.IsInRole(UserRoles.WarrantyServiceRepresentative) ? user.EmployeeNumber : query.queryModel.SelectedEmployeeNumber;
            
            var model = new WSRCallSummaryModel()
            {
                EmployeeNumber = employeeNumber,
                EmployeeTiedToRepresentatives = GetEmployeesTiedToRepresentatives(user),
                ServiceCalls = GetWSRServiceCalls(employeeNumber).OrderBy(x => x.CommunityName).ThenBy(x => x.NumberOfDaysRemaining).ThenBy(x => x.HomeownerName),
            };

            model.EmployeeName = user.IsInRole(UserRoles.WarrantyServiceRepresentative) 
                ? user.UserName 
                : model.EmployeeTiedToRepresentatives.First(x => x.EmployeeNumber == model.EmployeeNumber).EmployeeName.ToTitleCase();

            return model;
        }

        private IEnumerable<ServiceCall> GetWSRServiceCalls(string employeeNumber)
        {
            using (_database)
            {
                const string sql = @"SELECT 
                                        sc.ServiceCallId as ServiceCallId
                                        , Servicecallnumber as CallNumber
                                        , sc.CreatedDate
                                        , sc.SpecialProject as IsSpecialProject
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
                                    AND sc.ServiceCallStatusId = @1
                                    ORDER BY cm.CommunityName, ho.HomeOwnerName, j.AddressLine";

                var serviceCalls = _database.FetchOneToMany<ServiceCall, ServiceCallLine>(x => x.ServiceCallId, sql, employeeNumber, ServiceCallStatus.Open.Value);
                
                foreach (var serviceCall in serviceCalls)
                {
                    const string notesSql = @"SELECT ServiceCallNote AS Note FROM ServiceCallNotes WHERE ServiceCallId = @0 AND (ServiceCallNote <> '' OR ServiceCallNote != null)";

                    serviceCall.ServiceCallNotes = _database.Fetch<ServiceCallNote>(notesSql, serviceCall.ServiceCallId);
                }

                return serviceCalls;
            }
        }

        private IEnumerable<WSRCallSummaryModel.EmployeeTiedToRepresentative> GetEmployeesTiedToRepresentatives(IUser user)
        {
            const string sql = @"SELECT DISTINCT e.EmployeeId as WarrantyRepresentativeEmployeeId, e.EmployeeNumber, LOWER(e.EmployeeName) as EmployeeName from CommunityAssignments ca
                                    INNER join Communities c
                                    ON ca.CommunityId = c.CommunityId
                                    INNER join Employees e
                                    ON ca.EmployeeId = e.EmployeeId
                                    INNER JOIN Cities ci
                                    ON c.CityId = ci.CityId
                                    WHERE CityCode IN ({0})
                                    ORDER BY EmployeeName";

            using (_database)
            {
                var result = _database.Fetch<WSRCallSummaryModel.EmployeeTiedToRepresentative>(string.Format(sql, user.Markets.CommaSeparateWrapWithSingleQuote()));

                return result;
            }
        }
    }
}