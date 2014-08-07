namespace Warranty.Core.DataAccess
{
    using System.Linq;
    using Extensions;
    using NHibernate.Cfg;
    using NHibernate.Tool.hbm2ddl;
    using Security;

    public static class ConfigurationFactoryExtensions
    {
        public static Configuration AttachAuditEventListeners(this Configuration config, IUserSession session)
        {
            var auditEventListener = new AuditEventListener(session);
            config.EventListeners.PreInsertEventListeners = config.EventListeners.PreInsertEventListeners.Concat(auditEventListener).ToArray();
            config.EventListeners.PreUpdateEventListeners = config.EventListeners.PreUpdateEventListeners.Concat(auditEventListener).ToArray();
            return config;
        }

        public static Configuration ValidateConfigurationSchema(this Configuration cfg)
        {
            new SchemaValidator(cfg).Validate();
            return cfg;
        }
    }
}