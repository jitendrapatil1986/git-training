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
                _database.Update(serviceCall);

                _bus.Send<NotifyServiceCallEscalatedStatusChanged>(x =>
                    {
                        x.ServiceCallId = serviceCall.ServiceCallId;
                        x.EscalatedDate = DateTime.UtcNow;
                        x.EscalatedReason = message.Text;
                    });

                var activityName = serviceCall.IsEscalated ? "Escalate" : "Deescalate";
                _logger.Write(activityName, message.Text, message.ServiceCallId, ActivityType.SpecialProject, ReferenceType.ServiceCall);

                var emails = GetEmails(serviceCall);

                return new ServiceCallToggleEscalateCommandResult
                    {
                        CallNumber = serviceCall.ServiceCallNumber,
                        IsEscalated = serviceCall.IsEscalated,
                        Emails = emails,
                        ShouldSendEmail = emails.Any()
                    };
            }
        }

        private List<string> GetEmails(ServiceCall serviceCall)
        {
            var emails = new List<string>();
            if (serviceCall.IsEscalated)
            {
                emails = QueryForWsrEmails(GetServiceCallMarket(serviceCall)).ToList();
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

        private static IEnumerable<string> QueryForWsrEmails(string market)
        {
            var query = new Common.Security.Queries.GetUsersByMarketAndRoleQuery(market, UserRoles.WarrantyCoordinatorRole);
            var result = query.Execute();
            return result.Select(x => x.Email);
        }
    }
}