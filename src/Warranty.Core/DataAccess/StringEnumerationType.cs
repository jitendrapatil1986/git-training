using NHibernate;
using NHibernate.Type;
using Warranty.Core.Enumerations;

namespace Warranty.Core.DataAccess
{
    public class StringEnumerationType<TEnumeration> : GenericCompositeUserType<TEnumeration>
        where TEnumeration : StringEnumeration<TEnumeration>
    {
        public StringEnumerationType()
            : base(false,
                new IType[] { NHibernateUtil.String, NHibernateUtil.String },
                new[] { "Value", "DisplayName" },
                vals =>
                {
                    TEnumeration value;

                    if (vals[0] == null)
                        return null;

                    return StringEnumeration<TEnumeration>.TryFromValue((string)vals[0], out value) ? value : null;
                },
                enumVal => enumVal)
        {
        }
    }
}