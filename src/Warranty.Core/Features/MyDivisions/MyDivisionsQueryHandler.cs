using System;
using System.Collections.Generic;
using System.Linq;
using Common.Security.Session;
using NPoco;
using Warranty.Core.Entities;
using Warranty.Core.Extensions;

namespace Warranty.Core.Features.MyDivisions
{
    public class MyDivisionsQueryHandler : IQueryHandler<MyDivisionsQuery, Dictionary<Guid, string>>
    {
        private readonly IUserSession _userSession;
        private readonly IDatabase _database;

        public MyDivisionsQueryHandler(IUserSession userSession, IDatabase database)
        {
            _userSession = userSession;
            _database = database;
        }

        public Dictionary<Guid, string> Handle(MyDivisionsQuery query)
        {
            var user = _userSession.GetCurrentUser();
            var markets = user.Markets;

            const string sql = @"SELECT DISTINCT
	                        d.DivisionId,
	                        d.DivisionName,
	                        d.DivisionCode
                        FROM 
	                        Communities com
	                        LEFT JOIN Cities c ON c.CityId = com.CityId
	                        LEFT JOIN Divisions d on d.DivisionId = com.DivisionId
                        WHERE d.DivisionId IS NOT NULL AND c.CityCode IN (@0)";

            var result = _database.Fetch<Division>(sql, user.Markets);
            var divisions = new Dictionary<Guid, string>();

            if (result == null || !result.Any())
                return divisions;

            foreach (var division in result)
            {
                divisions.Add(division.DivisionId, string.Format("({0}) {1}", division.DivisionCode, division.DivisionName));
            }

            return divisions;
        }
    }
}