namespace Warranty.Core.DataAccess.Mappings
{
    using Entities;

    public class ServiceCallNoteMap : AuditableEntityMap<ServiceCallNote>
    {
        public ServiceCallNoteMap()
        {
            TableName("ServiceCallNotes")
                .PrimaryKey("ServiceCallNoteId", false)
                .Columns(x =>
                {
                    x.Column(y => y.ServiceCallId);
                    x.Column(y => y.Note).WithName("ServiceCallNote");
                    x.Column(y => y.ServiceCallLineItemId);
                });
        }
    }
}
