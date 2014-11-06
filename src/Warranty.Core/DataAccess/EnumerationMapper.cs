using NPoco;
using Warranty.Core.Extensions;

namespace Warranty.Core.DataAccess
{
    public class EnumerationMapper : DefaultMapper
    {
        public override System.Func<object, object> GetToDbConverter(System.Type destType, System.Type SourceType)
        {
            if (SourceType.IsEnumeration())
                return x => SourceType.GetProperty("Value").GetValue(x, new object[] { });
            return base.GetToDbConverter(destType, SourceType);
        }

        public override System.Func<object, object> GetFromDbConverter(System.Type destType, System.Type sourceType)
        {
            if (destType.IsEnumeration())
                return x => destType.BaseType.GetMethod("FromInt32").Invoke(null, new[] { x });
            return base.GetFromDbConverter(destType, sourceType);
        }
    }
}