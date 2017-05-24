namespace Warranty.Core.Features.HelpDoc
{
    using NPoco;    
    using System.Collections.Generic;

    public class HelpDocHandler : IQueryHandler<HelpDocQuery,HelpDocModel>
    {
        private readonly IDatabase _database;

        public HelpDocHandler(IDatabase database)
        {
            _database = database;
        }

        public HelpDocModel Handle(HelpDocQuery query)
        {
            var model = new HelpDocModel()
            {
                DocumentFeatures = GetDocFeature()
            };
            return model;        
           
        }

        private List<HelpDocModel.HelpDocumentFeatures> GetDocFeature()
        {
            using (_database)
            {
                const string Sql = @"SELECT DocFeatureId, DocFeatureName FROM HelpDocumentFeatures order by DocFeatureName";

                var docfeatures = _database.Fetch<HelpDocModel.HelpDocumentFeatures>(Sql);

                foreach (var docfeature in docfeatures)
                {
                    const string featureItemSql = @"SELECT DocFeatureItemId, DocFeatureId, DocFeatureItemName FROM HelpDocumentFeatureItems WHERE DocFeatureId = @0";

                    docfeature.DocumentFeatureItems = _database.Fetch<HelpDocModel.HelpDocumentFeatureItems>(featureItemSql, docfeature.DocFeatureId);

                    foreach (var DocumentFeatureItem in docfeature.DocumentFeatureItems)
                    {
                        const string ItemUrlSql = @"SELECT UrlId,DocFeatureItemId,Url,ItemDocName FROM HelpDocumentItemsUrl WHERE DocFeatureItemId = @0";

                        DocumentFeatureItem.DocumentItemsUrl = _database.Fetch<HelpDocModel.HelpDocumentItemsUrl>(ItemUrlSql, DocumentFeatureItem.DocFeatureItemId);
                    }
                }

                return docfeatures;              

            }
        }
    }
}
