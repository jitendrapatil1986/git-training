namespace Warranty.Core.Configurations
{
    using System;
    using Newtonsoft.Json.Serialization;

    public class WarrantyContractResolver : CamelCasePropertyNamesContractResolver
    {
        protected override JsonContract CreateContract(Type objectType)
        {
            if (typeof(NHibernate.Proxy.INHibernateProxy).IsAssignableFrom(objectType))
                return base.CreateContract(objectType.BaseType);

            return base.CreateContract(objectType);
        }
    }
}