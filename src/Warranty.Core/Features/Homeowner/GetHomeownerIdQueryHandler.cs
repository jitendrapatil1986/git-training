using Accounting.API.Models;
using Warranty.Core.Services;

namespace Warranty.Core.Features.Homeowner
{
    public class GetHomeOwnerIdQueryHandler : IQueryHandler<GetHomeownerIdQuery, HomeownerIsPayableValidationResponseDto>
    {
        private readonly IAccountingService _accountingService;

        public GetHomeOwnerIdQueryHandler(IAccountingService accountingService)
        {
            _accountingService = accountingService;
        }

        public HomeownerIsPayableValidationResponseDto Handle(GetHomeownerIdQuery query)
        {
            return _accountingService.GetHomeownerIdIfValid(query.Name, query.Address);
        }
    }
}