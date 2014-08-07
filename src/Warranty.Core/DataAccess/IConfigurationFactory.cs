namespace Warranty.Core.DataAccess
{
    using NHibernate.Cfg;

    public interface IConfigurationFactory
    {
        Configuration CreateConfiguration();
    }
}