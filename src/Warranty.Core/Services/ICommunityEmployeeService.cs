namespace Warranty.Core.Services
{
    using System;

    public interface ICommunityEmployeeService
    {
        Guid? GetEmployeeIdForWsc(Guid jobId);
    }
}