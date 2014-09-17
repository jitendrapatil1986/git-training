namespace Warranty.UI.Core.Initialization
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Warranty.Core.Extensions;

    public class ModelMetadataProvider : DataAnnotationsModelMetadataProvider
    {
        protected override ModelMetadata CreateMetadata(IEnumerable<System.Attribute> attributes, System.Type containerType, System.Func<object> modelAccessor, System.Type modelType, string propertyName)
        {
            var metaData = base.CreateMetadata(attributes, containerType, modelAccessor, modelType, propertyName);
            if (metaData.DisplayName == null && metaData.PropertyName != null)
            {
                metaData.DisplayName = metaData.PropertyName.SplitTitleCase();
            }
            return metaData;
        }
    }
}