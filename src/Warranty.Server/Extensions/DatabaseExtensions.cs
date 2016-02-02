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
    }
}
