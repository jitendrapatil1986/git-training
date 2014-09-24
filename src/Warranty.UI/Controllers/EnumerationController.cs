using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Warranty.UI.Controllers
{
    using System.Reflection;
    using System.Web.Mvc;
    using Newtonsoft.Json.Linq;
    using Warranty.Core.Extensions;

    public class EnumerationController : Controller
    {
        public string Get(string id)
        {
            var typeName = string.Format("Warranty.Core.Enumerations.{0}, Warranty.Core", id);

            var type = Type.GetType(typeName);
            if (type == null)
                return "";

            var enumerations = type.GetMembers(BindingFlags.Public | BindingFlags.Static).ToArray();
            var props = type.GetMembers(BindingFlags.Public | BindingFlags.Instance).ToArray();

            var result = new JObject(
                from enumeration in enumerations
                where !enumeration.HasAttribute<Yay.Enumerations.DeprecatedAttribute>()
                let value = type.GetField(enumeration.Name).GetValue(null)
                select new JProperty(enumeration.Name,
                    new JObject(
                        from prop in props
                        let propertyInfo = type.GetProperty(prop.Name)
                        where propertyInfo != null
                        select new JProperty(prop.Name, propertyInfo.GetValue(value, null))
                        )
                    )
                );
            Response.ContentType = "application/javascript";
            return string.Format("define(function () {{ return {0}; }});", result.ToJson());
        }
    }
}