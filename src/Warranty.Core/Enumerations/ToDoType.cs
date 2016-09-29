namespace Warranty.Core.Enumerations
{
    using System.Collections.Generic;
    using System.Linq;

    public class ToDoType : Enumeration<ToDoType>
    {
        public static readonly ToDoType ServiceCallApproval = new ToDoType(1, "Service Call Approval",
                                                                           new List<string>
                                                                               {
                                                                                   UserRoles.WarrantyAdmin,
                                                                                   UserRoles.CustomerCareManager,
                                                                                   UserRoles.WarrantyServiceCoordinator
                                                                               });

        public static readonly ToDoType JobChangedTask = new ToDoType(2, "Job Changed Task",
                                                            new List<string>
                                                                {
                                                                    UserRoles.WarrantyServiceRepresentative,
                                                                });

        public static readonly ToDoType PaymentRequestApprovalUnderWarranty = new ToDoType(3, "Non Closed Out Payments",
                                                            new List<string>
                                                                {
                                                                    UserRoles.WarrantyAdmin,
                                                                    UserRoles.CustomerCareManager,
                                                                });
        
        public static readonly ToDoType CommunityEmployeeAssignment = new ToDoType(4, "Community Employee Assignment",
                                                            new List<string>
                                                                {
                                                                    UserRoles.WarrantyAdmin,
                                                                    UserRoles.CustomerCareManager,
                                                                    UserRoles.WarrantyServiceCoordinator,
                                                                });

        public static readonly ToDoType JobAnniversaryTask = new ToDoType(5, "Job Anniversary",
                                                    new List<string>
                                                                {
                                                                    UserRoles.CustomerCareManager,
                                                                    UserRoles.WarrantyServiceCoordinator,
                                                                    UserRoles.WarrantyServiceRepresentative,
                                                                });

        public static readonly ToDoType PaymentRequestApprovalOutOfWarranty = new ToDoType(6, "Closed Out Payments",
                                                    new List<string>
                                                                {
                                                                    UserRoles.WarrantyAdmin,
                                                                    UserRoles.CustomerCareManager,
                                                                });

        public static readonly ToDoType PaymentStatusChanged = new ToDoType(7, "Payment Status Changed",
                                            new List<string>
                                                                {
                                                                    UserRoles.WarrantyServiceRepresentative,
                                                                });

        public static readonly ToDoType ServiceCallClosure = new ToDoType(8, "Service Call Closure",
           new List<string>
                                                                               {
                                                                                   UserRoles.WarrantyAdmin,
                                                                                   UserRoles.CustomerCareManager,
                                                                                   UserRoles.WarrantyServiceCoordinator
                                                                               });

        public static readonly ToDoType WarrantyOrientationApproval = new ToDoType(9, "Warranty Orientation Approval",
                                                            new List<string>
                                                                {
                                                                    UserRoles.WarrantyAdmin,
                                                                    UserRoles.CustomerCareManager,
                                                                    UserRoles.WarrantyServiceCoordinator,
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
