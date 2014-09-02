﻿using System;
using System.Linq;
using System.Collections.Generic;
using NPoco;
using Warranty.Core.Enumerations;
using Warranty.Core.Extensions;
using Warranty.Core.Security;
using Warranty.Core.ToDoInfrastructure.ConcreteTodos;
using Warranty.Core.ToDoInfrastructure.Interfaces;
using Warranty.Core.ToDoInfrastructure.Models;

namespace Warranty.Core.ToDoInfrastructure
{
    public class ToDoAggregator : IToDoAggregator
    {
        private readonly IDatabase _database;
        private readonly IUserSession _userSession;

        public ToDoAggregator(IDatabase database, IUserSession userSession)
        {
            _database = database;
            _userSession = userSession;
        }

        public List<IToDo> Execute()
        {
            var user = _userSession.GetCurrentUser();
            var serviceCallApprovalToDos = GetServiceCallApprovalToDos(user);
            //var escalationApprovalToDos = GetEscalationApprovalToDos();
            //var paymentRequestApprovalToDos = GetPaymentRequestApprovalToDos();

            var toDos = new List<IToDo>();

            toDos.AddRange(serviceCallApprovalToDos);
            //toDos.AddRange(escalationApprovalToDos);
            //toDos.AddRange(paymentRequestApprovalToDos);

            return toDos.OrderBy(x => x.Date).ToList();
        }

        private IEnumerable<IToDo> GetServiceCallApprovalToDos(IUser user)
        {
            var userMarkets = user.Markets;
            const string sql = @"SELECT
                                         wc.CreatedDate as [Date]							 
                                        ,ho.HomeOwnerName
                                        ,j.AddressLine
                                        ,wc.ServiceCallId
                                        ,wc.ServiceCallNumber                                
                                        ,j.JobId
                                        ,j.JobNumber
                                        ,DATEDIFF(yy, j.CloseDate, wc.CreatedDate) as YearsWithinWarranty
                                        ,j.CloseDate as WarrantyStartDate							
                                    FROM 
                                        [ServiceCalls] wc
                                    INNER join Jobs j
                                        ON wc.JobId = j.JobId
                                    INNER join HomeOwners ho
                                        ON j.CurrentHomeOwnerId = ho.HomeOwnerId                                  
                                    INNER join Employees e
                                        ON wc.WarrantyRepresentativeEmployeeId = e.EmployeeId
                                    INNER JOIN Communities cm
                                        ON j.CommunityId = cm.CommunityId
                                    INNER JOIN Cities ci
                                        ON cm.CityId = ci.CityId
                                    where 
                                        wc.ServiceCallStatusId = @0    
                                    and 
                                        ci.CityCode in ({0})";

            var query = string.Format(sql, userMarkets.CommaSeparateWrapWithSingleQuote());
            var toDos = _database.Fetch<ToDoServiceCallApproval, ToDoServiceCallApprovalModel>(query, ServiceCallStatus.Requested.Value);

            return toDos;
        }

        //private IEnumerable<IToDo> GetEscalationApprovalToDos()
        //{
        //    //TODO: Not the final query
        //    var todo = new ToDoEscalationApproval()
        //    {
        //        Model = new ToDoEscalationApprovalModel
        //        {
        //            HomeOwnerAddress = "Address",
        //            HomeOwnerName = "Name",
        //            EscalationRequestedBy = "John S"
        //        },
        //        Date = DateTime.Now
        //    };

        //    yield return todo;
        //}

        //private IEnumerable<IToDo> GetPaymentRequestApprovalToDos()
        //{
        //    //TODO: Not the final query
        //    var todo = new ToDoPaymentRequestApproval()
        //    {
        //        Model = new ToDoPaymentRequestApprovalModel()
        //        {
        //            HomeOwnerAddress = "Address",
        //            HomeOwnerName = "Name",
        //            PaymentAmount = (decimal)150.55
        //        },
        //        Date = DateTime.Now
        //    };

        //    yield return todo;
        //}
    }
}