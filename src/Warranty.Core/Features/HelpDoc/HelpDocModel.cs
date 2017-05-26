using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warranty.Core.Features.HelpDoc
{
    public class HelpDocModel
    {
        public List<HelpDocumentFeatures> DocumentFeatures { get; set; }        

        public class HelpDocumentFeatures
        {
            public int DocFeatureId { get; set; }
            public string DocFeatureName { get; set; }
            public List<HelpDocumentFeatureItems> DocumentFeatureItems { get; set; }
        }

        public class HelpDocumentFeatureItems
        {
            public int DocFeatureItemId { get; set; }
            public int DocFeatureId { get; set; }
            public string DocFeatureItemName { get; set; }
            public List<HelpDocumentItemsUrl> DocumentItemsUrl { get; set; }

        }

        public class HelpDocumentItemsUrl
        {
            public int UrlId { get; set; }
            public int DocFeatureItemId { get; set; }
            public string Url { get; set; }
            public string ItemDocName { get; set; }

        }
    }
}
