namespace Warranty.Core.Features.HelpDocument
{
    using NPoco;
    using System.Collections.Generic;
    using System.Linq;

    public class HelpDocumentHandler : IQueryHandler<HelpDocumentQuery, HelpDocumentModel>
    {
        private readonly IDatabase _database;

        public HelpDocumentHandler(IDatabase database)
        {
            _database = database;
        }

        public HelpDocumentModel Handle(HelpDocumentQuery query)
        {
            var model = new HelpDocumentModel()
            {
                DocumentFeatures = GetDocFeature()
            };
            return model;

        }

        private List<HelpDocumentModel.HelpDocumentFeature> GetDocFeature()
        {
            using (_database)
            {
                const string Sql = @"SELECT DocumentFeatureId, DocumentFeatureName FROM HelpDocumentFeatures order by DocumentFeatureName";
                const string featureItemSql = @"SELECT DocumentFeatureItemId, DocumentFeatureId, DocumentFeatureItemName, Url FROM HelpDocumentFeatureItems";

                var features = _database.Fetch<HelpDocumentModel.HelpDocumentFeature>(Sql);
                features.ForEach(feature =>
                {
                    feature.DocumentFeatureItems = new List<HelpDocumentModel.HelpDocumentFeatureItem>();
                });
                var items = _database.Fetch<HelpDocumentModel.HelpDocumentFeatureItem>(featureItemSql);
                items.ForEach(item =>
                {
                    var feature = features.SingleOrDefault(f => f.DocumentFeatureId == item.DocumentFeatureId);
                    if (feature != null)
                    {                       
                        feature.DocumentFeatureItems.Add(item);
                    }
                });

                return features;
            }
        }
    }
}
