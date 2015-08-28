namespace Warranty.Core.Features.DeleteServiceCall
{
    using System;

    public class AdminDeleteServiceCallCommand : ICommand
    {
        public Guid ServiceCallId { get; set; }
    }
}