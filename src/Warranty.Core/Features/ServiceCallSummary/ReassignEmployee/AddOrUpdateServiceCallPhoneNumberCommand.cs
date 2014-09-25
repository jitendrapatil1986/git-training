namespace Warranty.Core.Features.ServiceCallSummary.ReassignEmployee
{
    public class AddOrUpdateServiceCallPhoneNumberCommand : InlineEditCommandBase
    {
        public int PhoneNumberTypeValue { get; set; }
    }
}