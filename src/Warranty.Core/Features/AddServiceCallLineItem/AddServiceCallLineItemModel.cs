namespace Warranty.Core.Features.AddServiceCallLineItem
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;

    public class AddServiceCallLineItemModel
    {
        public Guid ServiceCallId { get; set; }
        public string ProblemCode { get; set; }
        public string ProblemDescription { get; set; }
    }
}
