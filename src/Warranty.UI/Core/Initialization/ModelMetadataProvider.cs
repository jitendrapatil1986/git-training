using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace Warranty.UI.Core.Initialization
{
    public class ModelMetadataProvider : DataAnnotationsModelMetadataProvider
    {
        private static readonly Regex PascalCaseToSpacesRegex = new Regex(@"(\B[A-Z]+?(?=[A-Z][^A-Z])|\B[A-Z]+?(?=[^A-Z]))", RegexOptions.Compiled);

        protected override ModelMetadata CreateMetadata(IEnumerable<System.Attribute> attributes, System.Type containerType, System.Func<object> modelAccessor, System.Type modelType, string propertyName)
        {
            var metaData = base.CreateMetadata(attributes, containerType, modelAccessor, modelType, propertyName);
            if (metaData.DisplayName == null && metaData.PropertyName != null)
            {
                metaData.DisplayName = PascalCaseToSpacesRegex.Replace(metaData.PropertyName, " $1");
            }
            return metaData;
        }
    }
}