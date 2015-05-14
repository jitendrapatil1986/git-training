using Warranty.Core.Configurations;

namespace Warranty.Core.Features.AddServiceCallPayment
{
    using System;
    using Entities;
    using Enumerations;
    using InnerMessages;
    using NPoco;
    using NServiceBus;
    using Common.Security.User.Session;
    using Services;

    public class AddStandAloneBackchargeCommandHandler : ICommandHandler<AddStandAloneBackchargeCommand, AddStandAloneBackchargeCommandDto>
    {
        private readonly IDatabase _database;
        private readonly IUserSession _userSession;
        private readonly IResolveObjectAccount _resolveObjectAccount;
        private readonly IBus _bus;

        public AddStandAloneBackchargeCommandHandler(IDatabase database, IUserSession userSession, IBus bus, IResolveObjectAccount resolveObjectAccount)
        {
            _database = database;
            _userSession = userSession;
            _bus = bus;
            _resolveObjectAccount = resolveObjectAccount;
        }

        public AddStandAloneBackchargeCommandDto Handle(AddStandAloneBackchargeCommand message)
        {
            using (_database)
            {
                var currentUser = _userSession.GetCurrentUser();

                var job = _database.Single<Job>(@"SELECT j.*
                                                            FROM ServiceCallLineItems scli
                                                            INNER JOIN ServiceCalls sc
                                                                ON sc.ServiceCallId = scli.ServiceCallId
                                                            INNER JOIN Jobs j
                                                                ON J.JobId = sc.JobId
                                                            WHERE scli.ServiceCallLineItemId = @0", message.ServiceCallLineItemId);

                var serviceCall = _database.Single<ServiceCall>(@"SELECT sc.* 
                                                                    FROM ServiceCallLineItems scli
                                                            INNER JOIN ServiceCalls sc
                                                                ON sc.ServiceCallId = scli.ServiceCallId
                                                            WHERE scli.ServiceCallLineItemId = @0", message.ServiceCallLineItemId);

                var rootProblem = _database.Single<string>("SELECT RootProblem FROM ServiceCallLineItems WHERE ServiceCallLineItemId=@0", message.ServiceCallLineItemId);

                var costCode = RootProblem.FromDisplayName(rootProblem).CostCode;

                var community = _database.SingleOrDefaultById<Community>(job.CommunityId);

                var city = _database.SingleOrDefaultById<City>(community.CityId);

                var communityNumber = community.CommunityNumber;

                if (job.IsOutOfWarranty)
                {
                    communityNumber = WarrantyConfigSection.GetCity(city.CityCode.ToUpper()).ClosedOutCommunity;
                }

                var backcharge = new Backcharge
                {
                    PaymentId = null,
                    JobNumber = job.JobNumber,
                    ServiceCallLineItemId = message.ServiceCallLineItemId,
                    BackchargeVendorNumber = message.BackchargeVendorNumber,
                    BackchargeVendorName = message.BackchargeVendorName,
                    BackchargeAmount = message.BackchargeAmount,
                    BackchargeReason = message.BackchargeReason,
                    PersonNotified = message.PersonNotified,
                    PersonNotifiedPhoneNumber = message.PersonNotifiedPhoneNumber,
                    PersonNotifiedDate = message.PersonNotifiedDate,
                    BackchargeResponseFromVendor = message.BackchargeResponseFromVendor,
                    CostCode = costCode.CostCode,
                    BackchargeStatus = BackchargeStatus.Requested,
                    Username = currentUser.LoginName,
                    EmployeeNumber = currentUser.EmployeeNumber,
                    ObjectAccount = _resolveObjectAccount.ResolveLaborObjectAccount(job, serviceCall),
                    CommunityNumber = string.IsNullOrEmpty(communityNumber) ? string.Empty : communityNumber.Substring(0, 4)
                };
                _database.Insert(backcharge);

                _bus.Send<NotifyBackchargeRequested>(x =>
                {
                    x.BackchargeId = backcharge.BackchargeId;
                    x.Username = backcharge.Username;
                    x.EmployeeNumber = backcharge.EmployeeNumber;
                });

                return new AddStandAloneBackchargeCommandDto { CostCode = costCode, BackchargeId = backcharge.BackchargeId };
            }
        }
    }
}