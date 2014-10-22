namespace Warranty.Core.Features.JobSummary.ChangeHomeowner
{
    using NPoco;

    public class ChangeHomeownerQueryHandler : IQueryHandler<ChangeHomeownerQuery, ChangeHomeownerModel>
    {
        private readonly IDatabase _database;

        public ChangeHomeownerQueryHandler(IDatabase database)
        {
            _database = database;
        }

        public ChangeHomeownerModel Handle(ChangeHomeownerQuery query)
        {
            using (_database)
            {
                const string jobSql = @"SELECT TOP 1 j.[JobId]
                                        ,j.[JobNumber]
                                        ,j.[CloseDate]
                                        ,j.[AddressLine]
                                        ,j.[City]
                                        ,j.[StateCode]
                                        ,j.[PostalCode]
                                        ,j.[CurrentHomeOwnerId]
                                        ,j.[WarrantyExpirationDate]
                                        , DATEDIFF(yy, j.CloseDate, getdate()) as YearsWithinWarranty
                                        , j.CloseDate as WarrantyStartDate
                                        ,ho.HomeOwnerName as CurrentHomeownerName
                                        ,ho.HomeOwnerNumber as CurrentHomeownerHomeownerNumber
                                        ,ho.HomePhone as CurrentHomeownerHomePhone
                                        ,ho.OtherPhone as CurrentHomeownerOtherPhone
                                        ,ho.WorkPhone1 as CurrentHomeownerWorkNumber
                                        ,ho.EmailAddress as CurrentHomeownerEmailAddress
                                    FROM Jobs j
                                    INNER JOIN HomeOwners ho
                                    ON j.JobId = ho.JobId
                                    WHERE j.JobId = @0
                                    ORDER BY ho.HomeownerNumber DESC";

                var changeHomeownerModel = _database.Single<ChangeHomeownerModel>(jobSql, query.JobId);

                return changeHomeownerModel;
            }
        }
    }
}