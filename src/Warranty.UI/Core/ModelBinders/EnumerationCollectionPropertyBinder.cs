namespace Warranty.UI.Core.ModelBinders
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Reflection;
    using System.Web.Mvc;
    using Yay.Enumerations;
    using Yay.Enumerations.MVC.ModelBinders;

    public class EnumerationCollectionPropertyBinder : FilteredPropertyBinderBase
	{
		public override bool ShouldBind(ControllerContext controllerContext, ModelBindingContext bindingContext, PropertyDescriptor propertyDescriptor)
		{
			var isCollection = propertyDescriptor.PropertyType.IsGenericType && propertyDescriptor.PropertyType.GetGenericTypeDefinition().Equals(typeof (ICollection<>));
			if (!isCollection)
				return false;

			return propertyDescriptor.PropertyType.GetGenericArguments()[0].Closes(typeof(Enumeration<>));
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
				
				var ids = value.AttemptedValue.Split(',');

				var enumerationType = propertyDescriptor.PropertyType.GetGenericArguments()[0];

				var listConstructor = typeof (List<>).MakeGenericType(enumerationType).GetConstructor(new Type[0]);

				var items = (IList) listConstructor.Invoke(new object[0]);

				foreach (var id in ids)
				{
					items.Add(GetEnumeration(enumerationType, id));
				}

				return items;
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
				var fromValueOrDefaultMethod = enumerationType.GetMethod("FromValueOrDefault", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy, null, new []{typeof(int)}, null);
				
				return fromValueOrDefaultMethod.Invoke(null, new object[] {listItemValue});
			}

			var fromDisplayNameOrDefaultMethod = enumerationType.GetMethod("FromDisplayNameOrDefault", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy, null, new []{typeof(string)}, null);

			return fromDisplayNameOrDefaultMethod.Invoke(null, new object[] {value});
		}
	}
}