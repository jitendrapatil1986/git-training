using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warranty.Core.Features.HelpDocument
{
    public class HelpDocumentModel
    {
        public List<HelpDocumentFeature> DocumentFeatures { get; set; }        

        public class HelpDocumentFeature
        {
            public int DocumentFeatureId { get; set; }
            public string DocumentFeatureName { get; set; }
            public List<HelpDocumentFeatureItem> DocumentFeatureItems { get; set; }
        }

        public class HelpDocumentFeatureItem
        {
            public int DocumentFeatureItemId { get; set; }
            public int DocumentFeatureId { get; set; }
            public string DocumentFeatureItemName { get; set; }           
            public string Url { get; set; }

        }        
    }
}
