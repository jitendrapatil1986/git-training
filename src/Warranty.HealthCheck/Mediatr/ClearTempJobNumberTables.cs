using MediatR;
using Warranty.HealthCheck.Data;

namespace Warranty.HealthCheck.Mediatr
{
    public class ClearTempJobNumberTablesRequestHandler : RequestHandler<ClearTempJobNumberTablesRequest>
    {
        private readonly IWarrantyDatabase _warrantyDatabase;

        public ClearTempJobNumberTablesRequestHandler(IWarrantyDatabase warrantyDatabase)
        {
            _warrantyDatabase = warrantyDatabase;
        }

        protected override void HandleCore(ClearTempJobNumberTablesRequest message)
        {
            _warrantyDatabase.Execute("DELETE FROM HEALTH_SoldJob");
        }
    }

    public class ClearTempJobNumberTablesRequest : IRequest
    {
        
    }
}