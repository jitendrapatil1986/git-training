using NHibernate.Cfg;

namespace Warranty.Core.DataAccess
{
    public interface IConfigurationFactory
    {
        Configuration CreateConfiguration();
    }
}