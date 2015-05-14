using System;
using NPoco;
using Warranty.Core.Entities;
using Common.Security.User.Session;

namespace Warranty.Core.DataAccess
{
    public class SqlServerDatabase : Database
    {
        private readonly IUserSession _userSession;

        public SqlServerDatabase(string connectionString, DatabaseType databaseType, IUserSession userSession)
            : base(connectionString, databaseType)
        {
            KeepConnectionAlive = false;
            _userSession = userSession;
        }

        protected override bool OnUpdating(UpdateContext updateContext)
        {
            var auditableEntity = updateContext.Poco as IAuditableEntity;

            if (auditableEntity != null)
            {
                auditableEntity.UpdatedDate = DateTime.UtcNow;
                auditableEntity.UpdatedBy = _userSession.GetCurrentUser().UserName;
            }

            return base.OnUpdating(updateContext);
        }

        protected override bool OnInserting(InsertContext insertContext)
        {
            SetPrimaryKey(insertContext);

            var auditableEntity = insertContext.Poco as IAuditableEntity;

            if (auditableEntity != null)
            {
                auditableEntity.CreatedDate = DateTime.UtcNow;
                auditableEntity.CreatedBy = _userSession.GetCurrentUser().UserName;
            }

            var auditCreatedEntity = insertContext.Poco as IAuditCreatedEntity;

            if (auditCreatedEntity != null)
            {
                auditCreatedEntity.CreatedDate = DateTime.UtcNow;
                auditCreatedEntity.CreatedBy = _userSession.GetCurrentUser().UserName;
            }

            return base.OnInserting(insertContext);
        }

        private static void SetPrimaryKey(InsertContext insertContext)
        {
            var entity = insertContext.Poco;
            var primaryKey = entity.GetType().GetProperty(insertContext.PrimaryKeyName);
            if (primaryKey != null && primaryKey.PropertyType == typeof(Guid))
            {
                if ((Guid)primaryKey.GetValue(entity) == Guid.Empty)
                {
                    primaryKey.SetValue(entity, GuidComb.Generate());
                }
            }
        }
    }
}
