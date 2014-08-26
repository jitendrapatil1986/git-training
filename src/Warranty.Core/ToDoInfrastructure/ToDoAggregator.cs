using System;
using System.Linq;
using System.Collections.Generic;
using Warranty.Core.Enumerations;
using Warranty.Core.Security;
using Warranty.Core.ToDoInfrastructure.ConcreteTodos;
using Warranty.Core.ToDoInfrastructure.Interfaces;
using Warranty.Core.ToDoInfrastructure.Models;

namespace Warranty.Core.ToDoInfrastructure
{
    public class ToDoAggregator : IToDoAggregator
    {
        private readonly IUserSession _userSession;

        public ToDoAggregator(IUserSession userSession)
        {
            _userSession = userSession;
        }

        public List<IToDo> Execute()
        {
            var serviceCallApprovalToDos = GetServiceCallApprovalToDos();
            var escalationApprovalToDos = GetEscalationApprovalToDos();
            var paymentRequestApprovalToDos = GetPaymentRequestApprovalToDos();

            var toDos = new List<IToDo>();

            toDos.AddRange(serviceCallApprovalToDos);
            toDos.AddRange(escalationApprovalToDos);
            toDos.AddRange(paymentRequestApprovalToDos);

            return toDos.OrderByDescending(x => x.Date).ToList();
        }

        private IEnumerable<IToDo> GetServiceCallApprovalToDos()
        {
            //TODO: Not the final query
            var todo = new ToDoServiceCallApproval
            {
                Model = new ToDoServiceCallApprovalModel
                {
                    HomeOwnerAddress = "Address",
                    HomeOwnerName = "Name",
                    YearsWithinWarranty = 10
                },
                Date = DateTime.Now,
            };

            yield return todo;
        }

        private IEnumerable<IToDo> GetEscalationApprovalToDos()
        {
            //TODO: Not the final query
            var todo = new ToDoEscalationApproval()
            {
                Model = new ToDoEscalationApprovalModel
                {
                    HomeOwnerAddress = "Address",
                    HomeOwnerName = "Name",
                    EscalationRequestedBy = "John S"
                },
                Date = DateTime.Now
            };

            yield return todo;
        }

        private IEnumerable<IToDo> GetPaymentRequestApprovalToDos()
        {
            //TODO: Not the final query
            var todo = new ToDoPaymentRequestApproval()
            {
                Model = new ToDoPaymentRequestApprovalModel()
                {
                    HomeOwnerAddress = "Address",
                    HomeOwnerName = "Name",
                    PaymentAmount = (decimal)150.55
                },
                Date = DateTime.Now
            };

            yield return todo;
        }
    }
}