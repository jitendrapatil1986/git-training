namespace Warranty.Core.Features.DeleteServiceCall
{
    using System;

    public class DeleteServiceCallCommand : ICommand
    {
        public Guid ServiceCallId { get; set; }
    }
}