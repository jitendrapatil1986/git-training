using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warranty.Core.DataAccess.Mappings
{
    using Entities;

    public class ServiceCallLineItemMap : AuditableEntityMap<ServiceCallLineItem>
    {
        public ServiceCallLineItemMap()
        {
            TableName("ServiceCallLineItems")
                .PrimaryKey("ServiceCallLineItemId", false)
                .Columns(x =>
                {
                    x.Column(y => y.ServiceCallId);
                    x.Column(y => y.LineNumber);
                    x.Column(y => y.ProblemCode);
                    x.Column(y => y.ProblemDescription);
                    x.Column(y => y.CauseDescription);
                    x.Column(y => y.ClassificationNote);
                    x.Column(y => y.LineItemRoot);
                    x.Column(y => y.Completed);
                });
        }
    }
}