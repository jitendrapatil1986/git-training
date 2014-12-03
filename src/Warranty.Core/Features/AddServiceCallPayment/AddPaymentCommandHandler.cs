namespace Warranty.Core.Features.AddServiceCallPayment
{
    using System;
    using Configurations;
    using Entities;
    using Enumerations;
    using InnerMessages;
    using NPoco;
    using NServiceBus;
    using Security;
    using Services;

    public class AddPaymentCommandHandler : ICommandHandler<AddPaymentCommand, Guid>
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

        public Guid Handle(AddPaymentCommand message)
        {
            using (_database)
            {
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

                
                var payment = new Payment
                {
                    Amount = message.Amount,
                    InvoiceNumber = message.InvoiceNumber,
                    ServiceCallLineItemId = message.ServiceCallLineItemId,
                    PaymentStatus = PaymentStatus.Requested,
                    VendorNumber = message.VendorNumber,
                    VendorName = message.VendorName,
                    JobNumber = job.JobNumber,
                    CommunityNumber = string.IsNullOrEmpty(job.JobNumber) ? "" : job.JobNumber.Substring(0, 4),
                    CostCode = WarrantyCostCode.FromValue(message.SelectedCostCode).CostCode,
                    ObjectAccount = _resolveObjectAccount.ResolveLaborObjectAccount(job, serviceCall),
                };

                _database.Insert(payment);

                var currentUser = _userSession.GetCurrentUser();    

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
                        CostCode = WarrantyCostCode.FromValue(message.SelectedCostCode).CostCode,
                        BackchargeStatus = BackchargeStatus.Requested,
                        Username = currentUser.LoginName,
                        EmployeeNumber = currentUser.EmployeeNumber
                    };
                    _database.Insert(backcharge);
                }

                return payment.PaymentId;
            }
        }
    }
}