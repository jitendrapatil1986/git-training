namespace Warranty.Core.Features.ServiceCallSummary.ServiceCallLineItem
{
    using System;
    using System.Collections.Generic;
    using Enumerations;
    using NPoco;
    using Security;

    public class ServiceCallLineItemQueryHandler : IQueryHandler<ServiceCallLineItemQuery, ServiceCallLineItemModel>
    {
        private readonly IDatabase _database;
        private readonly IUserSession _userSession;

        public ServiceCallLineItemQueryHandler(IDatabase database, IUserSession userSession)
        {
            _database = database;
            _userSession = userSession;
        }

        public ServiceCallLineItemModel Handle(ServiceCallLineItemQuery query)
        {
            var user = _userSession.GetCurrentUser();
            var model = GetServiceCallLineItem(query.ServiceCallLineItemId);

            model.ServiceCallLineItemNotes = GetServiceCallLineNotes(query.ServiceCallLineItemId);
            model.ServiceCallLineItemAttachments = GetServiceCallLineAttachments(query.ServiceCallLineItemId);
            model.ProblemCodes = SharedQueries.ProblemCodes.GetProblemCodeList(_database);
            model.CanReopenLines = user.IsInRole(UserRoles.WarrantyServiceManager) || user.IsInRole(UserRoles.WarrantyServiceCoordinator);

            return model;
        }

        private ServiceCallLineItemModel GetServiceCallLineItem(Guid serviceCallLineItemId)
        {
            const string sql = @"SELECT li.[ServiceCallLineItemId],
                                    li.[ServiceCallId],
                                    sc.[ServiceCallNumber],
                                    li.[LineNumber],
                                    li.[ProblemCode],
                                    li.[ProblemDescription],
                                    li.[CauseDescription],
                                    li.[ClassificationNote],
                                    li.[LineItemRoot],
                                    li.[CreatedDate],
                                    li.[ServiceCallLineItemStatusId] as ServiceCallLineItemStatus,
                                    li.[RootCause],
                                    li.[ProblemJdeCode],
                                    li.[ProblemDetailCode]
                                FROM ServiceCallLineItems li
                                INNER JOIN ServiceCalls sc
                                ON li.ServiceCallId = sc.ServiceCallId
                                WHERE ServiceCallLineItemId = @0";

            var result = _database.Single<ServiceCallLineItemModel>(sql, serviceCallLineItemId);

            return result;
        }

        private IEnumerable<ServiceCallLineItemModel.ServiceCallLineItemAttachment> GetServiceCallLineAttachments(Guid serviceCallLineItemId)
        {
            const string sql = @"SELECT [ServiceCallAttachmentId]
                                        ,[ServiceCallLineItemId]
                                        ,[DisplayName]
                                        ,[CreatedDate]
                                        ,[CreatedBy]
                                FROM [ServiceCallAttachments]
                                WHERE ServiceCallLineItemId = @0 AND IsDeleted=0";

            var result = _database.Fetch<ServiceCallLineItemModel.ServiceCallLineItemAttachment>(sql, serviceCallLineItemId.ToString());

            return result;
        }

        private IEnumerable<ServiceCallLineItemModel.ServiceCallLineItemNote> GetServiceCallLineNotes(Guid serviceCallLineItemId)
        {
            const string sql = @"SELECT [ServiceCallNoteId]
                                      ,[ServiceCallId]
                                      ,[ServiceCallNote] as Note
                                      ,[ServiceCallLineItemId]
                                      ,[CreatedDate]
                                      ,[CreatedBy]
                                FROM [ServiceCallNotes]
                                WHERE ServiceCallLineItemId = @0";

            var result = _database.Fetch<ServiceCallLineItemModel.ServiceCallLineItemNote>(sql, serviceCallLineItemId.ToString());

            return result;
        }
    }
}