using System;
using Warranty.Core.Entities;

namespace Warranty.Core.Features.Homeowner
{
    public class CreateNewHomeOwnerCommand : ICommand<HomeOwner>
    {
        public Guid JobId { get; set; }
        public string HomeOwnerName { get; set; }
        public string HomePhone { get; set; }
        public string OtherPhone { get; set; }
        public string WorkPhone1 { get; set; }
        public string WorkPhone2 { get; set; }
        public string EmailAddress { get; set; }
    }
}