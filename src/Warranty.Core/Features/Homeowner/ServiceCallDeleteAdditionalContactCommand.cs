namespace Warranty.Core.Features.Homeowner
{
    using System;

    public class ServiceCallDeleteAdditionalContactCommand : ICommand
    {
        public Guid Id { get; set; }
    }
}