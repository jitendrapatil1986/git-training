namespace Warranty.Core.Features.HelpDocument
{
    using NPoco;    
    using System.Collections.Generic;

    public class HelpDocumentHandler : IQueryHandler<HelpDocumentQuery,HelpDocumentModel>
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

                var docfeatures = _database.Fetch<HelpDocumentModel.HelpDocumentFeature>(Sql);

                foreach (var docfeature in docfeatures)
                {
                    const string featureItemSql = @"SELECT hdfi.DocumentFeatureItemId, hdfi.DocumentFeatureId,hdfi. DocumentFeatureItemName, hdfu.Url FROM HelpDocumentFeatureItems hdfi
                                                    INNER JOIN HelpDocumentItemsUrls hdfu on hdfi.DocumentFeatureItemId = hdfu.DocumentFeatureItemId WHERE DocumentFeatureId = @0";

                    docfeature.DocumentFeatureItems = _database.Fetch<HelpDocumentModel.HelpDocumentFeatureItem>(featureItemSql, docfeature.DocumentFeatureId);
                }

                return docfeatures;              

            }
        }
    }
}
