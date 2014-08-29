using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warranty.Core.Features.CreateServiceCall
{
    using Entities;
    using Enumerations;
    using NPoco;
    using Security;

    public class CreateServiceCallCommandHandler: ICommandHandler<CreateServiceCallModel, Guid>
    {
        private readonly IDatabase _database;
        private readonly IUserSession _userSession;

        public CreateServiceCallCommandHandler(IDatabase database, IUserSession userSession)
        {
            _database = database;
            _userSession = userSession;
        }

        public Guid Handle(CreateServiceCallModel message)
        {
            var user = _userSession.GetCurrentUser();
            
            using (_database)
            {
                const string sql = @"SELECT TOP 1 ServiceCallNumber
                                    FROM ServiceCalls
                                    ORDER BY ServiceCallNumber DESC";

                var lastCall = _database.First<int>(sql);
                var newCallNumber = lastCall + 1;

                const string sql3 = @"SELECT *
                                        FROM Employees
                                        WHERE EmployeeNumber = @0";

                var employee = _database.First<Employee>(sql3, user.EmployeeNumber);
                var newCallId = Guid.NewGuid(); 

                var serviceCall = new ServiceCall
                {
                    ServiceCallId = newCallId,
                    ServiceCallNumber = newCallNumber,
                    ServiceCallStatus = ServiceCallStatus.Requested, //TODO: Set to requested for now until we figure out who is WSR and WC. ServiceCallStatus.Open,
                    JobId = message.JobId,
                    Contact = message.Contact,
                    WarrantyRepresentativeEmployeeId = employee.EmployeeId,
                    ServiceCallType = message.ServiceCallType,
                    CompletionDate = null,
                    CreatedBy = employee.Name,
                    CreatedDate = DateTime.Now,
                };

                _database.Insert(serviceCall);

                if (message.ServiceCallLineItems != null)
                {
                    foreach (var line in message.ServiceCallLineItems)
                    {
                        var serviceCallLine = new ServiceCallLineItem
                            {
                                ServiceCallId = newCallId,
                                ServiceCallLineItemId = Guid.NewGuid(),
                                LineNumber = line.LineItemNumber,
                                ProblemCode = line.ProblemCodeDisplayName,
                                ProblemDescription = line.ProblemDescription,
                                CreatedBy = employee.Name,
                                CreatedDate = DateTime.Now,
                            };

                        _database.Insert(serviceCallLine);
                    }
                }

                //TODO: Remove if not saving header notes.
                if (message.ServiceCallHeaderComments != null)
                {
                    foreach (var noteLine in message.ServiceCallHeaderComments)
                    {
                        var serviceCallComment = new ServiceCallComment
                            {
                                ServiceCallId = newCallId,
                                ServiceCallCommentId = Guid.NewGuid(),
                                Comment = noteLine.Comment,
                                CreatedBy = employee.Name,
                                CreatedDate = DateTime.Now,
                            };

                        _database.Insert(serviceCallComment);
                    }
                }

                return newCallId;
            }
        }
    }
}
