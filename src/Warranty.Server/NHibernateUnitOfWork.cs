using System;
using System.Transactions;
using NHibernate;
using NServiceBus.UnitOfWork;
using IsolationLevel = System.Data.IsolationLevel;

namespace Warranty.Server
{
    public class NHibernateUnitOfWork : IManageUnitsOfWork
    {
        public NHibernateUnitOfWork(ISession session)
        {
            Session = session;
        }

        public ISession Session { get; set; }

        public void Begin()
        {
            if (Session != null)
            {
                Session.BeginTransaction(GetIsolationLevel());
            }
        }

        public void End(Exception ex = null)
        {
            if (Session == null)
            {
                return;
            }

            using (Session)
            using (Session.Connection)
            using (Session.Transaction)
            {
                if (!Session.Transaction.IsActive)
                {
                    return;
                }

                if (ex != null)
                {
                    // Due to a race condition in NH3.3, explicit rollback can cause exceptions and corrupt the connection pool. 
                    // Especially if there are more than one NH session taking part in the DTC transaction
                    //currentSession.Transaction.Rollback();
                }
                else
                {
                    Session.Transaction.Commit();
                }
            }
        }

        IsolationLevel GetIsolationLevel()
        {
            if (Transaction.Current == null)
            {
                return IsolationLevel.Unspecified;
            }

            switch (Transaction.Current.IsolationLevel)
            {
                case System.Transactions.IsolationLevel.Chaos:
                    return IsolationLevel.Chaos;
                case System.Transactions.IsolationLevel.ReadCommitted:
                    return IsolationLevel.ReadCommitted;
                case System.Transactions.IsolationLevel.ReadUncommitted:
                    return IsolationLevel.ReadUncommitted;
                case System.Transactions.IsolationLevel.RepeatableRead:
                    return IsolationLevel.RepeatableRead;
                case System.Transactions.IsolationLevel.Serializable:
                    return IsolationLevel.Serializable;
                case System.Transactions.IsolationLevel.Snapshot:
                    return IsolationLevel.Snapshot;
                case System.Transactions.IsolationLevel.Unspecified:
                    return IsolationLevel.Unspecified;
                default:
                    return IsolationLevel.Unspecified;
            }
        }
    }
}