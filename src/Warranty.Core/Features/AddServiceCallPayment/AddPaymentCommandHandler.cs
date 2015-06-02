namespace Warranty.Core.Features.AddServiceCallPayment
{
    using Configurations;
    using Entities;
    using Enumerations;
    using InnerMessages;
    using NPoco;
    using NServiceBus;
    using Security;
    using Services;
    using System.Linq;

    public class AddPaymentCommandHandler : ICommandHandler<AddPaymentCommand, AddPaymentCommandDto>
    {
        private readonly IDatabase _database;
        private readonly IBus _bus;
        private readonly IUserSession _userSession;
        private readonly IResolveObjectAccount _resolveObjectAccount;

        public AddPaymentCommandHandler(IDatabase database, IBus bus, IUserSession userSession, IResolveObjectAccount resolveObjectAccount)
        {
            _database = database;
            _bus = bus;
            _userSession = userSession;
            _resolveObjectAccount = resolveObjectAccount;
        }

        public AddPaymentCommandDto Handle(AddPaymentCommand message)
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

                var communityNumber = job.JobNumber;  //community is first 4 chs of job but accounting needs job and pulls substring.

                if (job.IsOutOfWarranty)
                {
                    communityNumber = WarrantyConfigSection.GetCity(currentUser.Markets.FirstOrDefault()).ClosedOutCommunity;
                }

                var payment = new Payment
                {
                    Amount = message.Amount,
                    InvoiceNumber = message.InvoiceNumber,
                    Comments = message.Comments,
                    ServiceCallLineItemId = message.ServiceCallLineItemId,
                    PaymentStatus = PaymentStatus.Requested,
                    VendorNumber = message.VendorNumber,
                    VendorName = message.VendorName,
                    JobNumber = job.JobNumber,
                    CommunityNumber = communityNumber,
                    CostCode = costCode.CostCode,
                    ObjectAccount = _resolveObjectAccount.ResolveLaborObjectAccount(job, serviceCall),
                };

                _database.Insert(payment);

                _bus.Send<NotifyPaymentRequested>(x =>
                {
                    x.PaymentId = payment.PaymentId;
                    x.Username = currentUser.LoginName;
                });

                if (message.IsBackcharge)
                {
                    var backcharge = new Backcharge
                    {
                        PaymentId = payment.PaymentId,

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
                        EmployeeNumber = currentUser.EmployeeNumber
                    };
                    _database.Insert(backcharge);

                    return new AddPaymentCommandDto { CostCode = costCode, PaymentId = payment.PaymentId, BackchargeId = backcharge.BackchargeId};
                }

                return new AddPaymentCommandDto {CostCode = costCode, PaymentId = payment.PaymentId};
            }
        }
    }
}