using System;
using Warranty.Core.Entities;

namespace Warranty.Core.Features.Homeowner
{
    public class GetHomeOwnerQuery : IQuery<HomeOwner>
    {
        public GetHomeOwnerQuery(string jobNumber)
        {
            JobNumber = jobNumber;
        }

        public string JobNumber { get; set; }
    }
}