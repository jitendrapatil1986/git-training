namespace Warranty.Core.Features.ServiceCallSummary.Attachments
{
    using System.Web;
    using Entities;
    using NPoco;

    public class ServiceCallDownloadAttachmentQueryHandler : IQueryHandler<ServiceCallDownloadAttachmentQuery, ServiceCallDownloadAttachmentModel>
    {
        private readonly IDatabase _database;

        public ServiceCallDownloadAttachmentQueryHandler(IDatabase database)
        {
            _database = database;
        }

        public ServiceCallDownloadAttachmentModel Handle(ServiceCallDownloadAttachmentQuery query)
        {
            using (_database)
            {
                var attachment = _database.SingleById<ServiceCallAttachment>(query.Id);
                return new ServiceCallDownloadAttachmentModel
                    {
                        Bytes = System.IO.File.ReadAllBytes(attachment.FilePath),
                        FileName = attachment.DisplayName,
                        MimeMapping = MimeMapping.GetMimeMapping(attachment.FilePath)
                    };

            }
        }
    }
}