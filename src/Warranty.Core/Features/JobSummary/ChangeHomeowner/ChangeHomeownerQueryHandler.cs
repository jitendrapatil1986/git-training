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
                const string jobSql = @"SELECT j.[JobId]
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
                                    FROM Jobs j
                                    WHERE j.JobId = @0";

                var jobDetails = _database.Single<ChangeHomeownerModel>(jobSql, query.JobId);

                const string sql = @"SELECT ho.HomeOwnerName
                                        ,ho.HomeOwnerNumber
                                        ,ho.HomePhone as HomePhone
                                        ,ho.OtherPhone as OtherPhone
                                        ,ho.WorkPhone1 as WorkNumber
                                        ,ho.EmailAddress
                                    FROM Jobs j
                                    INNER JOIN HomeOwners ho
                                    ON j.JobId = ho.JobId
                                    WHERE j.JobId = @0";

                var currentHomeownerResult = _database.Single<ChangeHomeownerModel.CurrentHomeownerDetail>(sql, query.JobId);

                var model = new ChangeHomeownerModel
                    {
                        JobId = jobDetails.JobId,
                        JobNumber = jobDetails.JobNumber,
                        CloseDate = jobDetails.CloseDate,
                        AddressLine = jobDetails.AddressLine,
                        City = jobDetails.City,
                        StateCode = jobDetails.StateCode,
                        WarrantyStartDate = jobDetails.WarrantyStartDate,
                        YearsWithinWarranty = jobDetails.YearsWithinWarranty,
                        CurrentHomeowner = currentHomeownerResult,
                    };

                return model;
            }
        }
    }
}