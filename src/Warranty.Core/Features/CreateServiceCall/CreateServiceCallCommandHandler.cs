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


        public class ServiceCalls
        {
            public Guid ServiceCallId { get; set; }
            public int ServiceCallNumber { get; set; }
            public string ServiceCallType { get; set; }
            //public bool IsSpecialProject { get; set; }
            //public ServiceCallStatus ServiceCallStatus { get; set; }
            //public bool IsEscalated { get; set; }

            public int ServiceCallStatusId { get; set; }

            public Guid JobId { get; set; }
            public string Contact { get; set; }
            public Guid WarrantyRepresentativeEmployeeId { get; set; }
            public DateTime? CompletionDate { get; set; }
            public string WorkSummary { get; set; }
            public string HomeOwnerSignature { get; set; }
            public DateTime? CreatedDate { get; set; }
            public string CreatedBy { get; set; }
            public DateTime? UpdatedDate { get; set; }
            public string UpdatedBy { get; set; }
        }

        public class ServiceCallLineItems
        {
            public Guid ServiceCallLineItemId { get; set; }
            public Guid ServiceCallId { get; set; }
            public int LineNumber { get; set; }
            public string ProblemCode { get; set; }
            public string ProblemDescription { get; set; }
            public string CauseDescription { get; set; }
            public string ClassificationNote { get; set; }
            public string LineItemRoot { get; set; }
            public bool Completed { get; set; }
            public DateTime? CreatedDate { get; set; }
            public string CreatedBy { get; set; }
            public DateTime? UpdatedDate { get; set; }
            public string UpdatedBy { get; set; }
        }

        public class ServiceCallComments
        {
            public Guid ServiceCallCommentId { get; set; }
            public Guid ServiceCallId { get; set; }
            public string ServiceCallComment { get; set; }
            public DateTime? CreatedDate { get; set; }
            public string CreatedBy { get; set; }
            public DateTime? UpdatedDate { get; set; }
            public string UpdatedBy { get; set; }
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

                //TODO: Not the final model data and may need to use Entity model instead of newly created class in file.
                //var serviceCall = new ServiceCall
                //{
                //    ServiceCallId = newCallId,
                //    ServiceCallNumber = newCallNumber,
                //    //ServiceCallStatusId = 2,  //TODO: Pull correct status code.
                //    JobId = message.JobId,
                //    Contact = message.Contact,
                //    WarrantyRepresentativeEmployeeId = employee.EmployeeId, //For now set call to user logged in.
                //    ServiceCallType = message.ServiceCallType,
                //    WorkSummary = "Testing",
                //    //CompletionDate = null,
                //    CreatedBy = employee.Name, //Doesn't pull Name bc of Employee entity mapping issue.
                //    CreatedDate = DateTime.Now,
                //};

                var serviceCall = new ServiceCalls
                    {
                        ServiceCallId = newCallId,
                        ServiceCallNumber = newCallNumber,
                        ServiceCallStatusId = 2,  //TODO: Pull correct status code.
                        JobId = message.JobId,
                        Contact = message.Contact,
                        WarrantyRepresentativeEmployeeId = employee.EmployeeId, //For now set call to user logged in.
                        ServiceCallType = message.ServiceCallType,
                        WorkSummary = "Testing",
                        CompletionDate = null,
                        CreatedBy = employee.Name, //Doesn't pull Name bc of Employee entity mapping issue.
                        CreatedDate = DateTime.Now,
                    };

                _database.Insert(serviceCall);

                if (message.ServiceCallLineItems != null)
                {
                    foreach (var line in message.ServiceCallLineItems)
                    {
                        //TODO: Not the final model data and may need to use Entity model instead of newly created class in file.
                        var serviceCallLine = new ServiceCallLineItems
                            {
                                ServiceCallId = newCallId,
                                ServiceCallLineItemId = Guid.NewGuid(),
                                ProblemCode = line.ProblemCodeId,
                                ProblemDescription = line.ProblemDescription,
                                CreatedBy = employee.Name,
                                CreatedDate = DateTime.Now,
                            };

                        _database.Insert(serviceCallLine);
                    }
                }

                if (message.ServiceCallHeaderComments != null)
                {
                    foreach (var noteLine in message.ServiceCallHeaderComments)
                    {
                        //TODO: Not the final model data and may need to use Entity model instead of newly created class in file.
                        var serviceCallComment = new ServiceCallComments
                            {
                                ServiceCallId = newCallId,
                                ServiceCallCommentId = Guid.NewGuid(),
                                ServiceCallComment = noteLine.Comment,
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
