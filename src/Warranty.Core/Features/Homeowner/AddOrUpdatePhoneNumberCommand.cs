namespace Warranty.Core.Features.Homeowner
{
    public class AddOrUpdatePhoneNumberCommand : InlineEditCommandBase
    {
        public int PhoneNumberTypeValue { get; set; }
    }
}