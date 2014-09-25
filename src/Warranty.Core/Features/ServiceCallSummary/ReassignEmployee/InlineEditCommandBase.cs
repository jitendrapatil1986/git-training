namespace Warranty.Core.Features.ServiceCallSummary.ReassignEmployee
{
    using System;

    public class InlineEditCommandBase : ICommand
    {
        public Guid Pk { get; set; }
        public string Value { get; set; }
    }
}