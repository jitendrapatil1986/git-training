using Warranty.Core.Entities;

namespace Warranty.Core.Services
{
    public interface IEmployeeService
    {
        string[] GetEmployeesInMarket();
        string GetEmployeeMarkets();

        Employee GetEmployeeByNumber(int? employeeNumber);

        Employee GetEmployeeByNumber(string employeeNumber);

        Employee GetWsrByCommunity(string communityNumber);
    }
}