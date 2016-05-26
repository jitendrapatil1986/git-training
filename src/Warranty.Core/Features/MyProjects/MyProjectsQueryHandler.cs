using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Security.Session;
using NPoco;
using Warranty.Core.Entities;
using Warranty.Core.Extensions;

namespace Warranty.Core.Features.MyProjects
{
    public class MyProjectsQueryHandler : IQueryHandler<MyProjectsQuery, Dictionary<Guid, string>>
    {
        private readonly IUserSession _userSession;
        private readonly IDatabase _database;

        public MyProjectsQueryHandler(IUserSession userSession, IDatabase database)
        {
            _userSession = userSession;
            _database = database;
        }

        public Dictionary<Guid, string> Handle(MyProjectsQuery query)
        {
            var user = _userSession.GetCurrentUser();

            var sql = new StringBuilder();
            sql.Append(@"SELECT DISTINCT
                                p.ProjectId,
                                p.ProjectName
                        FROM Projects p
	                    INNER JOIN Communities com on p.ProjectId = com.ProjectId
                        INNER JOIN Divisions d on com.DivisionId = d.DivisionId
	                    INNER JOIN Cities c ON c.CityId = com.CityId
                        WHERE c.CityCode IN (@0)");

            if (query.DivisionId.HasValue)
                sql.Append(" AND d.DivisionId = @1");

            var result = _database.Fetch<Project>(sql.ToString(), user.Markets, query.DivisionId);
            
            var projects = new Dictionary<Guid, string>();
            if (result == null || !result.Any())
                return projects;

            foreach (var project in result)
            {
                projects.Add(project.ProjectId, project.ProjectName);
            }

            return projects;
        }
        
    }
}