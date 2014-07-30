namespace Warranty.UI.Core.ModelBinders
{
    using System;
    using System.Reflection;
    using System.Web.Mvc;
    using Yay.Enumerations;
    using Yay.Enumerations.MVC.ModelBinders;

    public class EnumerationModelBinder : IFilteredModelBinder
	{
		public bool ShouldBind(ControllerContext controllerContext, ModelBindingContext bindingContext)
		{
			return bindingContext.ModelType.Closes(typeof (Enumeration<>));
		}

		public object GetModelValue(ControllerContext controllerContext, ModelBindingContext bindingContext)
		{
			try
			{
				var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

				if (value == null)
				{
					return null;
				}

				var enumeration = GetEnumeration(bindingContext.ModelType, value.AttemptedValue);
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