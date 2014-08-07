namespace Warranty.Core.DataAccess
{
    using System;
    using Entities;
    using Extensions;
    using NHibernate.Event;
    using Security;

    internal class AuditEventListener : IPreInsertEventListener, IPreUpdateEventListener
    {
        private readonly IUserSession _userSession;

        public AuditEventListener(IUserSession userSession)
        {
            _userSession = userSession;
        }

        public bool OnPreInsert(PreInsertEvent e)
        {
            var createdByIndex = e.Persister.PropertyNames.IndexOf("CreatedBy");
            var createdDateIndex = e.Persister.PropertyNames.IndexOf("CreatedDate");

            var type = e.Entity.GetType();

            if (type.IsAssignableTo<IAuditableEntity>())
            {
                var auditableEntity = (IAuditableEntity)e.Entity;

                e.State[createdByIndex] = auditableEntity.CreatedBy = GetUserFullName();
                e.State[createdDateIndex] = auditableEntity.CreatedDate = DateTime.UtcNow;
            }

            return false;
        }

        public string GetUserFullName()
        {
            var userFullName = _userSession.GetCurrentUser().UserName;
            return userFullName;
        }

        public bool OnPreUpdate(PreUpdateEvent e)
        {
            var updatedByIndex = e.Persister.PropertyNames.IndexOf("UpdatedBy");
            var updatedDateIndex = e.Persister.PropertyNames.IndexOf("UpdatedDate");

            var type = e.Entity.GetType();

            if (type.IsAssignableTo<IAuditableEntity>())
            {
                var auditableEntity = (IAuditableEntity)e.Entity;

                e.State[updatedByIndex] = auditableEntity.UpdatedBy = GetUserFullName();
                e.State[updatedDateIndex] = auditableEntity.UpdatedDate = DateTime.UtcNow;
            }

            return false;
        }
    }
}