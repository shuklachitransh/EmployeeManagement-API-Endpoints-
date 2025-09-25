using EmployeeManagement.Data;
using EmployeeManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly AppDbContext _context;

        public EmployeeService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Employee?> GetEmployeeByIdAsync(int id)
        {
            return await _context.tblEmployees.FindAsync(id);
        }

        public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
        {
            return await _context.tblEmployees.ToListAsync();
        }

        public async Task<PaginatedResult<Employee>> GetEmployeesPaginatedAsync(int page, int pageSize)
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;

            var totalCount = await _context.tblEmployees.CountAsync();
            var employees = await _context.tblEmployees
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResult<Employee>
            {
                Data = employees,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };
        }

        public async Task<Employee> CreateEmployeeAsync(EmployeeCreateDto employeeDto)
        {
            var employee = new Employee
            {
                EmployeeName = employeeDto.EmployeeName,
                Email = employeeDto.Email,
                Department = employeeDto.Department,
                Salary = employeeDto.Salary,
                Address1 = employeeDto.Address1,
                Address2 = employeeDto.Address2,
                Address3 = employeeDto.Address3,
                State = employeeDto.State,
                District = employeeDto.District,
                Pincode = employeeDto.Pincode
            };

            _context.tblEmployees.Add(employee);
            await _context.SaveChangesAsync();
            return employee;
        }

        public async Task<Employee?> UpdateEmployeeAsync(int id, EmployeeUpdateDto employeeDto)
        {
            var existing = await _context.tblEmployees.FindAsync(id);
            if (existing == null) return null;

            existing.EmployeeName = employeeDto.EmployeeName;
            existing.Email = employeeDto.Email;
            existing.Department = employeeDto.Department;
            existing.Salary = employeeDto.Salary;
            existing.Address1 = employeeDto.Address1;
            existing.Address2 = employeeDto.Address2;
            existing.Address3 = employeeDto.Address3;
            existing.State = employeeDto.State;
            existing.District = employeeDto.District;
            existing.Pincode = employeeDto.Pincode;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteEmployeeAsync(int id)
        {
            var employee = await _context.tblEmployees.FindAsync(id);
            if (employee == null) return false;

            _context.tblEmployees.Remove(employee);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Employee>> SearchEmployeesAsync(string name)
        {
            return await _context.tblEmployees
                .Where(e => e.EmployeeName.Contains(name))
                .ToListAsync();
        }

        public async Task<IEnumerable<Employee>> GetEmployeesByDepartmentAsync(string department)
        {
            return await _context.tblEmployees
                .Where(e => e.Department == department)
                .ToListAsync();
        }

        public async Task<IEnumerable<Employee>> GetEmployeesByStateAsync(string state)
        {
            return await _context.tblEmployees
                .Where(e => e.State == state)
                .ToListAsync();
        }

        public async Task<IEnumerable<Employee>> GetEmployeesBySalaryRangeAsync(decimal minSalary, decimal maxSalary)
        {
            return await _context.tblEmployees
                .Where(e => e.Salary >= minSalary && e.Salary <= maxSalary)
                .OrderBy(e => e.Salary)
                .ToListAsync();
        }

        public async Task<IEnumerable<Employee>> GetTopEarnersAsync(int count)
        {
            return await _context.tblEmployees
                .OrderByDescending(e => e.Salary)
                .Take(count)
                .ToListAsync();
        }

        public async Task<EmployeeStatisticsDto> GetEmployeeStatisticsAsync()
        {
            var totalEmployees = await _context.tblEmployees.CountAsync();
            var avgSalary = await _context.tblEmployees.AverageAsync(e => e.Salary);
            var maxSalary = await _context.tblEmployees.MaxAsync(e => e.Salary);
            var minSalary = await _context.tblEmployees.MinAsync(e => e.Salary);

            var departmentStats = await _context.tblEmployees
                .GroupBy(e => e.Department)
                .Select(g => new DepartmentCount { Department = g.Key, Count = g.Count() })
                .ToListAsync();

            var stateStats = await _context.tblEmployees
                .GroupBy(e => e.State)
                .Select(g => new StateCount { State = g.Key, Count = g.Count() })
                .ToListAsync();

            return new EmployeeStatisticsDto
            {
                TotalEmployees = totalEmployees,
                AverageSalary = Math.Round(avgSalary, 2),
                MaxSalary = maxSalary,
                MinSalary = minSalary,
                DepartmentBreakdown = departmentStats,
                StateBreakdown = stateStats
            };
        }

        public async Task<IEnumerable<string>> GetDepartmentsAsync()
        {
            return await _context.tblEmployees
                .Select(e => e.Department)
                .Distinct()
                .OrderBy(d => d)
                .ToListAsync();
        }

        public async Task<IEnumerable<string>> GetStatesAsync()
        {
            return await _context.tblEmployees
                .Select(e => e.State)
                .Distinct()
                .OrderBy(s => s)
                .ToListAsync();
        }

        public async Task<bool> CheckEmailExistsAsync(string email)
        {
            return await _context.tblEmployees
                .AnyAsync(e => e.Email == email);
        }

        public async Task<bool> BulkDeleteEmployeesAsync(int[] employeeIds)
        {
            var employees = await _context.tblEmployees
                .Where(e => employeeIds.Contains(e.Id))
                .ToListAsync();

            if (employees.Count == 0) return false;

            _context.tblEmployees.RemoveRange(employees);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> BulkUpdateDepartmentAsync(int[] employeeIds, string newDepartment)
        {
            var employees = await _context.tblEmployees
                .Where(e => employeeIds.Contains(e.Id))
                .ToListAsync();

            if (employees.Count == 0) return false;

            foreach (var employee in employees)
            {
                employee.Department = newDepartment;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Employee>> AdvancedSearchAsync(EmployeeSearchDto searchDto)
        {
            var query = _context.tblEmployees.AsQueryable();

            if (!string.IsNullOrEmpty(searchDto.Name))
                query = query.Where(e => e.EmployeeName.Contains(searchDto.Name));

            if (!string.IsNullOrEmpty(searchDto.Department))
                query = query.Where(e => e.Department == searchDto.Department);

            if (!string.IsNullOrEmpty(searchDto.State))
                query = query.Where(e => e.State == searchDto.State);

            if (searchDto.MinSalary.HasValue)
                query = query.Where(e => e.Salary >= searchDto.MinSalary.Value);

            if (searchDto.MaxSalary.HasValue)
                query = query.Where(e => e.Salary <= searchDto.MaxSalary.Value);

            return await query.ToListAsync();
        }

        public async Task<PaginatedResult<Employee>> AdvancedSearchPaginatedAsync(EmployeeSearchDto searchDto)
        {
            var query = _context.tblEmployees.AsQueryable();

            if (!string.IsNullOrEmpty(searchDto.Name))
                query = query.Where(e => e.EmployeeName.Contains(searchDto.Name));

            if (!string.IsNullOrEmpty(searchDto.Department))
                query = query.Where(e => e.Department == searchDto.Department);

            if (!string.IsNullOrEmpty(searchDto.State))
                query = query.Where(e => e.State == searchDto.State);

            if (searchDto.MinSalary.HasValue)
                query = query.Where(e => e.Salary >= searchDto.MinSalary.Value);

            if (searchDto.MaxSalary.HasValue)
                query = query.Where(e => e.Salary <= searchDto.MaxSalary.Value);

            var totalCount = await query.CountAsync();
            var employees = await query
                .Skip((searchDto.Page - 1) * searchDto.PageSize)
                .Take(searchDto.PageSize)
                .ToListAsync();

            return new PaginatedResult<Employee>
            {
                Data = employees,
                TotalCount = totalCount,
                Page = searchDto.Page,
                PageSize = searchDto.PageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / searchDto.PageSize)
            };
        }

        public async Task<string> ExportToCsvAsync()
        {
            var employees = await _context.tblEmployees.ToListAsync();
            
            var csv = "Id,EmployeeName,Email,Department,Salary,Address1,Address2,Address3,State,District,Pincode\n";
            foreach (var emp in employees)
            {
                csv += $"{emp.Id},{emp.EmployeeName},{emp.Email},{emp.Department},{emp.Salary},{emp.Address1},{emp.Address2},{emp.Address3},{emp.State},{emp.District},{emp.Pincode}\n";
            }

            return csv;
        }
    }
}
