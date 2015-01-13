namespace Warranty.Core.Features.QuickSearch
{
    using System.Collections.Generic;
    using NPoco;
    using Common.Extensions;

    public class QuickSearchEmployeesQueryHandler : IQueryHandler<QuickSearchEmployeesQuery, IEnumerable<QuickSearchEmployeeModel>>
    {
        private readonly IDatabase _database;

        public QuickSearchEmployeesQueryHandler(IDatabase database)
        {
            _database = database;
        }

        public IEnumerable<QuickSearchEmployeeModel> Handle(QuickSearchEmployeesQuery query)
        {
            using (_database)
            {
                const string sqlTemplate = @"SELECT TOP 20 EmployeeId as Id, EmployeeNumber as Number, EmployeeName as Name
                                FROM Employees
                                WHERE EmployeeName+EmployeeNumber LIKE '%'+@0+'%'
                                ORDER BY EmployeeName";

                var result = _database.Fetch<QuickSearchEmployeeModel>(sqlTemplate, query.Query);
                result.ForEach(x => x.Name = x.Name.ToTitleCase());
                return result;
            }
        }
    }
}