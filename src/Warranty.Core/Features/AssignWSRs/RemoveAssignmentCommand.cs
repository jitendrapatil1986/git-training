using System;

namespace Warranty.Core.Features.AssignWSRs
{
    public class RemoveAssignmentCommand : ICommand
    {
        public Guid AssignmentId { get; set; }
    }
}