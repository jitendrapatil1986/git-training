namespace Warranty.Commands
{
    using System.Collections.Generic;
    using NServiceBus;

    public class RequestServiceCall : ICommand
    {
        public string LocalId { get; set; }

        public string JobNumber { get; set; }
        public List<LineItem> LineItems { get; set; }

        public class LineItem
        {
            public int LineNumber { get; set; }
            public string ProblemDescription { get; set; }
        }
    }
}
