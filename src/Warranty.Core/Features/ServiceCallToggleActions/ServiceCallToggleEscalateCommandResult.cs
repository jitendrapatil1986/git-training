namespace Warranty.Core.Features.ServiceCallToggleActions
{
    using System.Collections.Generic;

    public class ServiceCallToggleEscalateCommandResult 
    {
        public ServiceCallToggleEscalateCommandResult()
        {
            Emails = new List<string>(0);
        }
        public int CallNumber { get; set; }
        public bool IsEscalated { get; set; }
        public IEnumerable<string> Emails { get; set; }
        public bool ShouldSendEmail { get; set; }
        public string Url { get; set; }
    }
}