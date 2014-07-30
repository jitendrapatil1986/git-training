namespace Warranty.UI.Core.ModelBinders
{
    using System;
    using System.ComponentModel;
    using System.Reflection;
    using System.Web.Mvc;
    using Yay.Enumerations;
    using Yay.Enumerations.MVC.ModelBinders;

    public class EnumerationPropertyBinder : FilteredPropertyBinderBase
	{
		public override bool ShouldBind(ControllerContext controllerContext, ModelBindingContext bindingContext, PropertyDescriptor propertyDescriptor)
		{
			return propertyDescriptor.PropertyType.Closes(typeof(Enumeration<>));
		}

		public override object GetPropertyValue(ControllerContext controllerContext, ModelBindingContext bindingContext, PropertyDescriptor propertyDescriptor)
		{
			try
			{
				var value = GetValue(bindingContext, propertyDescriptor);

				if (value == null)
				{
					return null;
				}

				var enumeration = GetEnumeration(propertyDescriptor.PropertyType, value.AttemptedValue);
				return enumeration;
			}
			catch (Exception ex)
			{
				var message = string.Format("Unable to locate a valid value for query string parameter '{0}'", bindingContext.ModelName);
				throw new ApplicationException(message, ex);
			}
		}

		private static object GetEnumeration(IReflect enumerationType, string value)
		{
			int listItemValue;

			if (int.TryParse(value, out listItemValue))
			{
				var fromValueOrDefaultMethod = enumerationType.GetMethod("FromValueOrDefault", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy, null, new[] { typeof(int) }, null);

				return fromValueOrDefaultMethod.Invoke(null, new object[] { listItemValue });
			}

			var fromDisplayNameOrDefaultMethod = enumerationType.GetMethod("FromDisplayNameOrDefault", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy, null, new[] { typeof(string) }, null);
			return fromDisplayNameOrDefaultMethod.Invoke(null, new object[] { value });
		}
	}
}