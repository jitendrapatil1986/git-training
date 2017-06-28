using NPoco;
using Warranty.Core.ActivityLogger;
using Warranty.Core.Entities;
using Warranty.Core.Enumerations;
using System.Linq;

namespace Warranty.Core.Features.ServiceCallToggleActions
{
    using System;
    using System.Collections.Generic;
    using InnerMessages;
    using NServiceBus;

    public class ServiceCallToggleEscalateCommandHandler : ICommandHandler<ServiceCallToggleEscalateCommand, ServiceCallToggleEscalateCommandResult>
    {
        private readonly IDatabase _database;
        private readonly IActivityLogger _logger;
        private readonly IBus _bus;

        public ServiceCallToggleEscalateCommandHandler(IDatabase database, IActivityLogger logger, IBus bus)
        {
            _database = database;
            _logger = logger;
            _bus = bus;
        }

        public ServiceCallToggleEscalateCommandResult Handle(ServiceCallToggleEscalateCommand message)
        {
            using (_database)
            {
                var serviceCall = _database.SingleOrDefaultById<ServiceCall>(message.ServiceCallId);
                serviceCall.IsEscalated = !serviceCall.IsEscalated;
                serviceCall.EscalationReason = (serviceCall.IsEscalated) ? message.Text : string.Empty;
                serviceCall.EscalationDate = (serviceCall.IsEscalated) ? DateTime.UtcNow : (DateTime?)null;
 
                _database.Update(serviceCall);

                _bus.Send<NotifyServiceCallEscalatedStatusChanged>(x =>
                    {
                        x.ServiceCallId = serviceCall.ServiceCallId;
                        x.EscalatedDate = serviceCall.EscalationDate;
                        x.EscalatedReason = serviceCall.EscalationReason;
                    });

                var activityName = serviceCall.IsEscalated ? "Escalate" : "Deescalate";
                _logger.Write(activityName, message.Text, message.ServiceCallId, ActivityType.SpecialProject, ReferenceType.ServiceCall);

                var emails = GetEmails(serviceCall);

                const string sql = @"SELECT ServiceCallId
                            , ServiceCallNumber as CallNumber
                            , WarrantyRepresentativeEmployeeId
                            , ho.HomeOwnerName 
                            , ho.HomePhone
                            , c.CommunityName
                            , j.AddressLine     
                            , e.EmployeeNumber     
                          FROM ServiceCalls sc
                          INNER JOIN Jobs j
                            ON sc.JobId = j.JobId
                          INNER JOIN communities c
                            ON c.CommunityId = j.CommunityId
                          INNER JOIN HomeOwners ho
                            ON j.CurrentHomeOwnerId = ho.HomeOwnerId
                          LEFT OUTER JOIN Employees e
                            ON e.EmployeeId = sc.WarrantyRepresentativeEmployeeId 
                          WHERE sc.ServiceCallId = @0";

                var model = _database.SingleOrDefault<ServiceCallToggleEscalateCommandResult>(sql, serviceCall.ServiceCallId);
                model.IsEscalated = serviceCall.IsEscalated;
                model.Emails = emails;
                model.ShouldSendEmail = emails.Any();

                const string sqlcomments = @"SELECT ServiceCallNote 
                                             FROM ServiceCallNotes 
                                             WHERE ServiceCallId = @0";

                model.Comments = _database.Fetch<string>(sqlcomments, serviceCall.ServiceCallId);

                const string sqlLineItems = @"SELECT LineNumber, ProblemCode, ProblemDescription
                                              FROM ServiceCallLineItems 
                                              WHERE ServiceCallId = @0";

                model.LineItems = _database.Fetch<ServiceCallToggleEscalateCommandResult.ServiceCallLine>(sqlLineItems, serviceCall.ServiceCallId);

                return model;
            }
        }

        private List<string> GetEmails(ServiceCall serviceCall)
        {
            var emails = new List<string>();
            if (serviceCall.IsEscalated)
            {
                emails = QueryForCcmEmails(GetServiceCallMarket(serviceCall)).ToList();
            }
            return emails;
        }

        private string GetServiceCallMarket(ServiceCall serviceCall)
        {
            const string sql = @"SELECT 
                                ci.CityCode
                            FROM [ServiceCalls] wc
                            INNER JOIN Jobs j
	                           on wc.JobId = j.JobId
                            INNER JOIN Communities cm
	                           ON j.CommunityId = cm.CommunityId
                            INNER JOIN Cities ci
	                           ON cm.CityId = ci.CityId
                            WHERE wc.ServiceCallId = @0";
            return _database.ExecuteScalar<string>(sql, serviceCall.ServiceCallId);
        }

        private static IEnumerable<string> QueryForCcmEmails(string market)
        {
            var query = new Common.Security.Queries.GetUsersByMarketAndRoleQuery(market, UserRoles.CustomerCareManagerRole);
            var result = query.Execute();
            return result.Select(x => x.Email);
        }
    }
}