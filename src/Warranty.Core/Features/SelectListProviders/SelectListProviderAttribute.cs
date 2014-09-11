namespace Warranty.Core.Features.SelectListProviders
{
    using System;
    using StructureMap;

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class SelectListProviderAttribute : Attribute
    {
        public ISelectListProvider Provider { get; private set; }
        
        public SelectListProviderAttribute(Type providerType)
        {
            if (typeof(ISelectListProvider).IsAssignableFrom(providerType))
            {
                Provider = (ISelectListProvider)ObjectFactory.GetInstance(providerType);
            }
            else
            {
                throw new ArgumentException("Provider type must be of type ISelectListProvider", "providerType");
            }
        }
    }
}
