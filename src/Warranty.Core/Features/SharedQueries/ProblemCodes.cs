namespace Warranty.Core.Features.SharedQueries
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using NPoco;

    public static class ProblemCodes
    {
        public static IEnumerable<SelectListItem> GetProblemCodeList(IDatabase database)
        {
            const string sql = @"SELECT  ProblemCodeId as Value
                                        ,ProblemCode as Text
                                FROM lookups.ProblemCodes
                                ORDER BY ProblemCode";

            return database.Fetch<SelectListItem>(sql);
        }
    }
}