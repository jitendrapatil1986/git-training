namespace Warranty.Core.Features.SharedQueries
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using NPoco;

    public static class ProblemCodes
    {
        public static IEnumerable<SelectListItem> GetProblemCodeList(IDatabase database)
        {
            const string sql = @"SELECT DISTINCT JdeCode as Value
                                        ,CategoryCode as Text
                                FROM ProblemCodes
                                ORDER BY CategoryCode";

            return database.Fetch<SelectListItem>(sql);

        }

    }
}