namespace Warranty.Core.Features.ServiceCallSummary.Attachments
{
    using Entities;
    using NPoco;

    public class ServiceCallRenameAttachmentCommandHandler : ICommandHandler<ServiceCallRenameAttachmentCommand>
    {
        private readonly IDatabase _datatabse;

        public ServiceCallRenameAttachmentCommandHandler(IDatabase datatabse)
        {
            _datatabse = datatabse;
        }

        public void Handle(ServiceCallRenameAttachmentCommand message)
        {
            using (_datatabse)
            {
                var attachment = _datatabse.SingleById<ServiceCallAttachment>(message.Pk);
                if (attachment != null)
                {
                    attachment.DisplayName = message.Value;
                    _datatabse.Update(attachment);

                }
            }
        }
    }
}