namespace Warranty.Core.Features.SharedQueries
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using NPoco;

    public static class RootCauses
    {
        public static IEnumerable<SelectListItem> GetRootCauseList(IDatabase database, string problemCode)
        {
            const string sql = @"SELECT  rc.RootCauseId as Value
                                        ,rc.RootCause as Text
                                FROM lookups.RootCauses
                                INNER JOIN ProblemCodeRootCauses prc ON prc.RootCauseId = rc.RootCauseId
                                INNER JOIN ProblemCode ON pc ON pc.ProblemCodeId = prc.ProblemCodeId
                                WHERE pc.ProblemCode = @0
                                ORDER BY RootCause";

            return database.Fetch<SelectListItem>(sql, problemCode);
        }
    }
}
