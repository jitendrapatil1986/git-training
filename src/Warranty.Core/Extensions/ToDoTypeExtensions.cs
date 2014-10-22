namespace Warranty.Core.Extensions
{
    using System.Collections.Generic;
    using System.Linq;
    using Enumerations;

    public static class ToDoTypeExtensions
    {
        public static bool HasAccess(this ToDoType toDoType, IEnumerable<string> roles)
        {
            return toDoType.UserRolesWithAccess.Intersect(roles).Any();
        }
    }
}