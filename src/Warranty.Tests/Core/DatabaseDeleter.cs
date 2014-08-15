namespace Warranty.Tests.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using NPoco;

    public class DatabaseDeleter
    {
        private static readonly string[] _ignoredTables = new[] { "sysdiagrams", "[RoundhousE].[ScriptsRun]", "[RoundhousE].[ScriptsRunErrors]", "[RoundhousE].[Version]" };
        private static string[] _tablesToDelete;
        private static string _deleteSql;
        private static readonly object _lockObj = new object();
        private static bool _initialized;

        public DatabaseDeleter(IDatabase database)
        {
            using (database)
            {
                database.Execute(@"ALTER TABLE HomeOwners DROP CONSTRAINT FK_HomeOwners_JobId");
                database.Execute(@"UPDATE HomeOwners SET JobId = NULL");
            }
            
            BuildDeleteTables(database);

            using (database)
            {
                database.Execute(@"ALTER TABLE HomeOwners ADD CONSTRAINT FK_HomeOwners_JobId
                                    FOREIGN KEY (JobId) REFERENCES Jobs(JobId)");
            }
        }

        private class Relationship
        {
            public string PrimaryKeyTable { get; private set; }
            public string ForeignKeyTable { get; private set; }
        }

        public virtual void DeleteAllData(IDatabase database)
        {
            using (database)
            {
                database.Execute(_deleteSql);
            }
        }

        public static string[] GetTables()
        {
            return _tablesToDelete;
        }

        private void BuildDeleteTables(IDatabase database)
        {
            if (!_initialized)
            {
                lock (_lockObj)
                {
                    if (!_initialized)
                    {
                        var allTables = GetAllTables(database);

                        var allRelationships = GetRelationships(database);

                        _tablesToDelete = BuildTableList(allTables, allRelationships);

                        _deleteSql = BuildTableSql(_tablesToDelete);

                        _initialized = true;
                    }
                }
            }
        }

        private static string BuildTableSql(IEnumerable<string> tablesToDelete)
        {
            string completeQuery = "";
            foreach (var tableName in tablesToDelete.Except(_ignoredTables))
            {
                completeQuery += String.Format("delete from [{0}];", tableName);
            }
            return completeQuery;
        }

        private static string[] BuildTableList(ICollection<string> allTables, ICollection<Relationship> allRelationships)
        {
            var tablesToDelete = new List<string>();

            while (allTables.Any())
            {
                var leafTables = allTables.Except(allRelationships.Select(rel => rel.PrimaryKeyTable)).ToArray();

                tablesToDelete.AddRange(leafTables);

                foreach (var leafTable in leafTables)
                {
                    allTables.Remove(leafTable);
                    var relToRemove = allRelationships.Where(rel => rel.ForeignKeyTable == leafTable).ToArray();
                    foreach (var rel in relToRemove)
                    {
                        allRelationships.Remove(rel);
                    }
                }
            }

            return tablesToDelete.ToArray();
        }

        private static IList<Relationship> GetRelationships(IDatabase database)
        {
            var sql = @"select
                            so_pk.name as PrimaryKeyTable
                            , so_fk.name as ForeignKeyTable
                        from
                            sysforeignkeys sfk
                            inner join sysobjects so_pk on sfk.rkeyid = so_pk.id
                            inner join sysobjects so_fk on sfk.fkeyid = so_fk.id
                            where so_pk.name <> so_fk.name 
                        order by
                                so_pk.name
                                , so_fk.name";

            using (database)
            {
                return database.Fetch<Relationship>(sql);
            }
        }

        private static IList<string> GetAllTables(IDatabase database)
        {
            var query = "select t.name from sys.tables t INNER JOIN sys.schemas s ON t.schema_id = s.schema_id WHERE s.name = 'dbo'";

            using (database)
            {
                return database.Fetch<string>(query);
            }
        }
    }
}