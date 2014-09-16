using Warranty.Core.Enumerations;
using Warranty.Core.ToDoInfrastructure.Models;

namespace Warranty.Core.ToDoInfrastructure.ConcreteTodos
{
    public class ToDoCommunityEmployeeAssignment : ToDo<ToDoCommunityEmployeeAssignmentModel>
    {
        public override string ViewName
        {
            get { return "_ToDoCommunityEmployeeAssignment"; }
        }

        public override ToDoType Type
        {
            get { return ToDoType.CommunityEmployeeAssignment; }
        }
    }
}