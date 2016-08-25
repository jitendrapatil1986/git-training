using System.Collections.Generic;
using Common.Extensions;
using NPoco;

namespace Warranty.Core.Features.QuickSearch
{
    public class QuickSearchProjectCoordinatorsQueryHandler : IQueryHandler<QuickSearchProjectCoordinatorsQuery, IEnumerable<QuickSearchProjectCoordinatorModel>>
    {
        public IEnumerable<QuickSearchProjectCoordinatorModel> Handle(QuickSearchProjectCoordinatorsQuery query)
        {
            using (var database = new Database("SecurityDB"))
            {
                const string sqlTemplate = @"SELECT DISTINCT users.name as Name, users.mail as Email
                                FROM FlattenedMembership groups
                                    JOIN Users users ON groups.UserSamAccountName = users.samAccountName
                                WHERE (groups.GroupSamAccountName = 'ROLE_ProjectCoordinators' or groups.GroupSamAccountName = 'ROLE_DivisionCoordinators')
                                    AND users.name like @0
                                ORDER BY users.name";

                var result = database.Fetch<QuickSearchProjectCoordinatorModel>(sqlTemplate, "%" + query.Query + "%");
                result.ForEach(x => x.Name = x.Name.ToTitleCase());
                return result;
            }
        }
    }
}