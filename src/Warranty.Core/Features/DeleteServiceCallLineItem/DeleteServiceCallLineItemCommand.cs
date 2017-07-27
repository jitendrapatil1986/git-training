using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warranty.Core.Features.DeleteServiceCallLineItem
{
    public class DeleteServiceCallLineItemCommand : ICommand
    {
        public Guid ServiceCallLineItemId { get; set; }

    }
}
