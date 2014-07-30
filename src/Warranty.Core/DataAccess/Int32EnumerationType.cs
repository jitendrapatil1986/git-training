using NHibernate;
using NHibernate.Type;
using Warranty.Core.Enumerations;

namespace Warranty.Core.DataAccess
{
    public class Int32EnumerationType<TEnumeration> : GenericCompositeUserType<TEnumeration>
        where TEnumeration : Int32Enumeration<TEnumeration>
    {
        public Int32EnumerationType()
            : base(false,
                   new IType[] { NHibernateUtil.Int32, NHibernateUtil.String },
                   new[] { "Value", "DisplayName" },
                   vals =>
                   {
                       TEnumeration value;

                       if (vals[0] == null)
                           return null;

                       return Int32Enumeration<TEnumeration>.TryFromInt32((int)vals[0], out value) ? value : null;
                   },
                   enumVal => enumVal)
        {
        }
    }
}