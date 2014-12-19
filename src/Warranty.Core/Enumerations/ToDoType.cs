﻿namespace Warranty.Core.Enumerations
{
    using System.Collections.Generic;
    using System.Linq;

    public class ToDoType : Enumeration<ToDoType>
    {
        public static readonly ToDoType ServiceCallApproval = new ToDoType(1, "Service Call Approval",
                                                                           new List<string>
                                                                               {
                                                                                   UserRoles.WarrantyAdmin,
                                                                                   UserRoles.WarrantyServiceManager,
                                                                                   UserRoles.WarrantyServiceCoordinator
                                                                               });

        public static readonly ToDoType JobChangedTask = new ToDoType(2, "Job Changed Task",
                                                            new List<string>
                                                                {
                                                                    UserRoles.WarrantyAdmin,
                                                                    UserRoles.WarrantyServiceManager,
                                                                    UserRoles.WarrantyServiceCoordinator,
                                                                    UserRoles.WarrantyServiceRepresentative,
                                                                });

        public static readonly ToDoType PaymentRequestApprovalUnderWarranty = new ToDoType(3, "Non Closed Out Payments",
                                                            new List<string>
                                                                {
                                                                    UserRoles.WarrantyAdmin,
                                                                    UserRoles.WarrantyServiceManager,
                                                                });
        
        public static readonly ToDoType CommunityEmployeeAssignment = new ToDoType(4, "Community Employee Assignment",
                                                            new List<string>
                                                                {
                                                                    UserRoles.WarrantyAdmin,
                                                                    UserRoles.WarrantyServiceManager,
                                                                    UserRoles.WarrantyServiceCoordinator,
                                                                });

        public static readonly ToDoType JobAnniversaryTask = new ToDoType(5, "Job Anniversary",
                                                    new List<string>
                                                                {
                                                                    UserRoles.WarrantyServiceManager,
                                                                    UserRoles.WarrantyServiceCoordinator,
                                                                });

        public static readonly ToDoType PaymentRequestApprovalOutOfWarranty = new ToDoType(6, "Closed Out Payments",
                                                    new List<string>
                                                                {
                                                                    UserRoles.WarrantyAdmin,
                                                                    UserRoles.WarrantyServiceManager,
                                                                });

        private ToDoType(int value, string displayName, IEnumerable<string> userRolesWithAccess)
            : base(value, displayName)
        {
            UserRolesWithAccess = userRolesWithAccess;
        }

        public IEnumerable<string> UserRolesWithAccess { get; set; }

        public static IEnumerable<ToDoType> GetAccesibleToDos(IEnumerable<string> userRoles)
        {
            return GetAll().Where(x => x.UserRolesWithAccess.Intersect(userRoles).Any()).OrderBy(x=>x.DisplayName);
        }
    }
}
