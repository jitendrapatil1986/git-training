using System;
using NPoco;
using Warranty.Core.Entities;
using Warranty.Core.Security;

namespace Warranty.Core.DataAccess
{
    public class SqlServerDatabase : Database
    {
        private readonly IUserSession _userSession;

        public SqlServerDatabase(string connectionString, DatabaseType databaseType, IUserSession userSession)
            : base(connectionString, databaseType)
        {
            _userSession = userSession;
        }

        protected override bool OnUpdating(UpdateContext updateContext)
        {
            var poco = updateContext.Poco as IAuditableEntity;

            if (poco != null)
            {
                poco.UpdatedDate = DateTime.Now;
                poco.UpdatedBy = _userSession.GetCurrentUser().UserName;
            }

            return base.OnUpdating(updateContext);
        }

        protected override bool OnInserting(InsertContext insertContext)
        {
            var poco = insertContext.Poco as IAuditableEntity;

            if (poco != null)
            {
                poco.CreatedDate = DateTime.Now;
                poco.CreatedBy = _userSession.GetCurrentUser().UserName;
            }

            return base.OnInserting(insertContext);
        }
    }
}