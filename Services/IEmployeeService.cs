using EmployeeManagement.Models;

namespace EmployeeManagement.Services
{
    public interface IEmployeeService
    {
        Task<Employee?> GetEmployeeByIdAsync(int id);
        Task<IEnumerable<Employee>> GetAllEmployeesAsync();
        Task<PaginatedResult<Employee>> GetEmployeesPaginatedAsync(int page, int pageSize);
        Task<Employee> CreateEmployeeAsync(EmployeeCreateDto employeeDto);
        Task<Employee?> UpdateEmployeeAsync(int id, EmployeeUpdateDto employeeDto);
        Task<bool> DeleteEmployeeAsync(int id);
        Task<IEnumerable<Employee>> SearchEmployeesAsync(string name);
        Task<IEnumerable<Employee>> GetEmployeesByDepartmentAsync(string department);
        Task<IEnumerable<Employee>> GetEmployeesByStateAsync(string state);
        Task<IEnumerable<Employee>> GetEmployeesBySalaryRangeAsync(decimal minSalary, decimal maxSalary);
        Task<IEnumerable<Employee>> GetTopEarnersAsync(int count);
        Task<EmployeeStatisticsDto> GetEmployeeStatisticsAsync();
        Task<IEnumerable<string>> GetDepartmentsAsync();
        Task<IEnumerable<string>> GetStatesAsync();
        Task<bool> CheckEmailExistsAsync(string email);
        Task<bool> BulkDeleteEmployeesAsync(int[] employeeIds);
        Task<bool> BulkUpdateDepartmentAsync(int[] employeeIds, string newDepartment);
        Task<IEnumerable<Employee>> AdvancedSearchAsync(EmployeeSearchDto searchDto);
        Task<PaginatedResult<Employee>> AdvancedSearchPaginatedAsync(EmployeeSearchDto searchDto);
        Task<string> ExportToCsvAsync();
    }
}
