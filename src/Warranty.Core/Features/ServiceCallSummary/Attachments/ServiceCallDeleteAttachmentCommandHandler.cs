namespace Warranty.Core.Features.ServiceCallSummary.Attachments
{
    using Entities;
    using NPoco;

    public class ServiceCallDeleteAttachmentCommandHandler : ICommandHandler<ServiceCallDeleteAttachmentCommand>
    {
        private readonly IDatabase _datatabse;

        public ServiceCallDeleteAttachmentCommandHandler(IDatabase datatabse)
        {
            _datatabse = datatabse;
        }

        public void Handle(ServiceCallDeleteAttachmentCommand message)
        {
            using (_datatabse)
            {
                var attachment = _datatabse.SingleById<ServiceCallAttachment>(message.Id);
                if (attachment != null)
                {
                    attachment.IsDeleted = true;
                    _datatabse.Update(attachment);

                }
            }
        }
    }
}