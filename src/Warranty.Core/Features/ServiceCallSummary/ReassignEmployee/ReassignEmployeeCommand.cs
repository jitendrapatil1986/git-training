namespace Warranty.Core.Features.ServiceCallSummary.ReassignEmployee
{
    using System;

    public class ReassignEmployeeCommand : ICommand
    {
        public Guid Pk { get; set; }
        public string Value { get; set; }
    }
}
