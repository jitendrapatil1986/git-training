using System;
using System.Linq;
using FluentValidation;
using NPoco;
using Warranty.Core.Entities;

namespace Warranty.Core.Features.AssignWSRs
{
    public class AssignWSRsValidator : AbstractValidator<AssignWSRsModel>
    {
        private readonly IDatabase _database;

        public AssignWSRsValidator(IDatabase database)
        {
            _database = database;

            RuleFor(x => x.SelectedCommunityId).NotEmpty();
            RuleFor(x => x.SelectedEmployeeId).NotEmpty();
            RuleFor(x => x.SelectedEmployeeId).Must(BeUnique).WithMessage("Assignments must be unique.");
        }

        private bool BeUnique(AssignWSRsModel model, Guid employeeId)
        {
            using (_database)
            {
                return
                    ! _database.FetchBy<CommunityAssignment>(
                        sql =>
                            sql.Where(ca => ca.CommunityId == model.SelectedCommunityId && ca.EmployeeId == employeeId))
                        .Any();
            }
        }
    }
}