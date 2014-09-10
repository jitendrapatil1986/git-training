namespace Warranty.Core.Entities.Lookups
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Extensions;

    public abstract class LookupEntity
    {
        public int Id { get; set; }
        public string DisplayName { get; set; }

        public static Type GetTypeFromName(string typeName)
        {
            var lookupType = Assembly.GetAssembly(typeof(LookupEntity)).GetTypes().Single(x => x.Name == typeName && x.IsAssignableTo<LookupEntity>());

            return lookupType;
        }
    }
}
