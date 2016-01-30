using System.Collections.Generic;
using NHibernate.Mapping;

namespace Warranty.Server.Extensions
{
    using Core.Entities;
    using NPoco;

    public static class DatabaseExtensions
    {
        public static T SingleOrDefaultByJdeId<T>(this IDatabase database, string jdeId) where T : IJdeEntity
        {
            return database.SingleOrDefault<T>("WHERE JdeIdentifier=@0", jdeId);
        }

        public static T SingleByJdeId<T>(this IDatabase database, string jdeId) where T : IJdeEntity
        {
            return database.Single<T>("WHERE JdeIdentifier=@0", jdeId);
        }

        public static bool ExistsByJdeId<T>(this IDatabase database, string jdeId) where T : IJdeEntity
        {
            return database.SingleOrDefaultByJdeId<T>(jdeId) != null;
        }

        public static Employee GetEmployeeByNumber(this IDatabase database, int? employeeId)
        {
            if (!employeeId.HasValue)
                return null;
            return database.SingleOrDefault<Employee>("WHERE EmployeeNumber = @0", employeeId.Value);
        }

        public static Community GetCommunityByNumber(this IDatabase database, string communityNumber)
        {
            return database.SingleOrDefault<Community>("WHERE CommunityNumber = @0", communityNumber);
        }

        public static Job GetJobByNumber(this IDatabase database, string jobNumber)
        {
            return database.SingleOrDefault<Job>("WHERE JobNumber = @0", jobNumber);
        }

        public static List<HomeOwner> GetHomeOwnersByJobNumber(this IDatabase database, string jobNumber)
        {
            return database.Fetch<HomeOwner>("SELECT h.* FROM Homeowners h inner join Jobs j on j.JobId = h.JobId WHERE j.JobNumber = @0", jobNumber);
        }
    }
}
