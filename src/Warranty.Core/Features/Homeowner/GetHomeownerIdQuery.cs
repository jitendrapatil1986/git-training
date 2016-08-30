using Accounting.API.Models;

namespace Warranty.Core.Features.Homeowner
{
    public class GetHomeownerIdQuery : IQuery<HomeownerIsPayableValidationResponseDto>
    {
        public string Name { get; set; }
        public string Address { get; set; }
    }
}
