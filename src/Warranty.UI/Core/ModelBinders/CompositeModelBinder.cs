namespace Warranty.UI.Core.ModelBinders
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Web.Mvc;
    using Yay.Enumerations.MVC.ModelBinders;

    public class CompositeModelBinder : DefaultModelBinder
	{
		private readonly IEnumerable<IFilteredPropertyBinder> _propertyBinders;
		private readonly IEnumerable<IFilteredModelBinder> _modelBinders;

		public CompositeModelBinder()
		{
			_propertyBinders = new IFilteredPropertyBinder[]
			                   	{
			                   		new EnumerationPropertyBinder(),
			                   		new EnumerationCollectionPropertyBinder(),
								};

			_modelBinders = new IFilteredModelBinder[]
			                	{
			                		new EnumerationModelBinder(),
									new EnumerationCollectionModelBinder(), 
			                   	};
		}

		public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
		{
			var matchingBinders = _modelBinders.Where(b => b.ShouldBind(controllerContext, bindingContext));
			if (matchingBinders.Any())
				return matchingBinders.Select(result => result.GetModelValue(controllerContext, bindingContext)).FirstOrDefault();

			return base.BindModel(controllerContext, bindingContext);
		}

		protected override void BindProperty(ControllerContext controllerContext, ModelBindingContext bindingContext, PropertyDescriptor propertyDescriptor)
		{
			var matchingBinders = _propertyBinders.Where(b => b.ShouldBind(controllerContext, bindingContext, propertyDescriptor));

			foreach (var result in matchingBinders.Select(filteredModelBinder => filteredModelBinder.GetPropertyValue(controllerContext, bindingContext, propertyDescriptor)))
			{
				propertyDescriptor.SetValue(bindingContext.Model, result);
				return;
			}

			base.BindProperty(controllerContext, bindingContext, propertyDescriptor);
		}
	}
}