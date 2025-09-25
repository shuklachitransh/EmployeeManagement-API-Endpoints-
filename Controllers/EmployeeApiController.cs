using EmployeeManagement.Data;
using EmployeeManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeApiController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EmployeeApiController(AppDbContext context)
        {
            _context = context;
        }

        // ✅ Add Employee
        [HttpPost("add")]
        public async Task<IActionResult> AddEmployee([FromBody] Employee emp)
        {
            if (emp == null)
                return BadRequest("Employee data is required.");

            _context.tblEmployees.Add(emp);
            await _context.SaveChangesAsync();
            return Ok(emp);
        }

        // ✅ Get All Employees
        [HttpGet("list")]
        public async Task<IActionResult> GetEmployees()
        {
            var employees = await _context.tblEmployees.ToListAsync();
            return Ok(employees);
        }

        // ✅ Update Employee
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateEmployee(int id, [FromBody] Employee emp)
        {
            var existing = await _context.tblEmployees.FindAsync(id);
            if (existing == null) return NotFound("Employee not found.");

            // Update all fields
            existing.EmployeeName = emp.EmployeeName;
            existing.Email = emp.Email;
            existing.Department = emp.Department;
            existing.Salary = emp.Salary;
            existing.Address1 = emp.Address1;
            existing.Address2 = emp.Address2;
            existing.Address3 = emp.Address3;
            existing.State = emp.State;
            existing.District = emp.District;
            existing.Pincode = emp.Pincode;

            await _context.SaveChangesAsync();
            return Ok(existing);
        }

        // ✅ Delete Employee
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var existing = await _context.tblEmployees.FindAsync(id);
            if (existing == null) return NotFound("Employee not found.");

            _context.tblEmployees.Remove(existing);
            await _context.SaveChangesAsync();
            return Ok($"Employee with ID {id} deleted successfully.");
        }

        // 🆕 Get Employee by ID
        [HttpGet("get/{id}")]
        public async Task<IActionResult> GetEmployee(int id)
        {
            var employee = await _context.tblEmployees.FindAsync(id);
            if (employee == null) return NotFound("Employee not found.");
            return Ok(employee);
        }

        // 🆕 Search Employees by Name
        [HttpGet("search")]
        public async Task<IActionResult> SearchEmployees([FromQuery] string name)
        {
            if (string.IsNullOrEmpty(name))
                return BadRequest("Search name is required.");

            var employees = await _context.tblEmployees
                .Where(e => e.EmployeeName.Contains(name))
                .ToListAsync();
            
            return Ok(employees);
        }

        // 🆕 Filter Employees by Department
        [HttpGet("filter/department")]
        public async Task<IActionResult> GetEmployeesByDepartment([FromQuery] string department)
        {
            if (string.IsNullOrEmpty(department))
                return BadRequest("Department is required.");

            var employees = await _context.tblEmployees
                .Where(e => e.Department == department)
                .ToListAsync();
            
            return Ok(employees);
        }

        // 🆕 Filter Employees by State
        [HttpGet("filter/state")]
        public async Task<IActionResult> GetEmployeesByState([FromQuery] string state)
        {
            if (string.IsNullOrEmpty(state))
                return BadRequest("State is required.");

            var employees = await _context.tblEmployees
                .Where(e => e.State == state)
                .ToListAsync();
            
            return Ok(employees);
        }

        // 🆕 Get Employees with Pagination
        [HttpGet("paginated")]
        public async Task<IActionResult> GetEmployeesPaginated([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;

            var totalCount = await _context.tblEmployees.CountAsync();
            var employees = await _context.tblEmployees
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = new
            {
                Data = employees,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };

            return Ok(result);
        }

        // 🆕 Get Employee Statistics
        [HttpGet("statistics")]
        public async Task<IActionResult> GetEmployeeStatistics()
        {
            var totalEmployees = await _context.tblEmployees.CountAsync();
            var avgSalary = await _context.tblEmployees.AverageAsync(e => e.Salary);
            var maxSalary = await _context.tblEmployees.MaxAsync(e => e.Salary);
            var minSalary = await _context.tblEmployees.MinAsync(e => e.Salary);

            var departmentStats = await _context.tblEmployees
                .GroupBy(e => e.Department)
                .Select(g => new { Department = g.Key, Count = g.Count() })
                .ToListAsync();

            var stateStats = await _context.tblEmployees
                .GroupBy(e => e.State)
                .Select(g => new { State = g.Key, Count = g.Count() })
                .ToListAsync();

            var result = new
            {
                TotalEmployees = totalEmployees,
                AverageSalary = Math.Round(avgSalary, 2),
                MaxSalary = maxSalary,
                MinSalary = minSalary,
                DepartmentBreakdown = departmentStats,
                StateBreakdown = stateStats
            };

            return Ok(result);
        }

        // 🆕 Get All Departments
        [HttpGet("departments")]
        public async Task<IActionResult> GetDepartments()
        {
            var departments = await _context.tblEmployees
                .Select(e => e.Department)
                .Distinct()
                .OrderBy(d => d)
                .ToListAsync();
            
            return Ok(departments);
        }

        // 🆕 Get All States
        [HttpGet("states")]
        public async Task<IActionResult> GetStates()
        {
            var states = await _context.tblEmployees
                .Select(e => e.State)
                .Distinct()
                .OrderBy(s => s)
                .ToListAsync();
            
            return Ok(states);
        }

        // 🆕 Bulk Delete Employees
        [HttpDelete("bulk-delete")]
        public async Task<IActionResult> BulkDeleteEmployees([FromBody] int[] employeeIds)
        {
            if (employeeIds == null || employeeIds.Length == 0)
                return BadRequest("Employee IDs are required.");

            var employees = await _context.tblEmployees
                .Where(e => employeeIds.Contains(e.Id))
                .ToListAsync();

            if (employees.Count == 0)
                return NotFound("No employees found with the provided IDs.");

            _context.tblEmployees.RemoveRange(employees);
            await _context.SaveChangesAsync();

            return Ok($"Successfully deleted {employees.Count} employees.");
        }

        // 🆕 Bulk Update Department
        [HttpPut("bulk-update-department")]
        public async Task<IActionResult> BulkUpdateDepartment([FromBody] BulkUpdateDepartmentRequest request)
        {
            if (request.EmployeeIds == null || request.EmployeeIds.Length == 0)
                return BadRequest("Employee IDs are required.");
            if (string.IsNullOrEmpty(request.NewDepartment))
                return BadRequest("New department is required.");

            var employees = await _context.tblEmployees
                .Where(e => request.EmployeeIds.Contains(e.Id))
                .ToListAsync();

            if (employees.Count == 0)
                return NotFound("No employees found with the provided IDs.");

            foreach (var employee in employees)
            {
                employee.Department = request.NewDepartment;
            }

            await _context.SaveChangesAsync();
            return Ok($"Successfully updated department for {employees.Count} employees.");
        }

        // 🆕 Advanced Search with Multiple Filters
        [HttpGet("advanced-search")]
        public async Task<IActionResult> AdvancedSearch([FromQuery] string? name, [FromQuery] string? department, 
            [FromQuery] string? state, [FromQuery] decimal? minSalary, [FromQuery] decimal? maxSalary)
        {
            var query = _context.tblEmployees.AsQueryable();

            if (!string.IsNullOrEmpty(name))
                query = query.Where(e => e.EmployeeName.Contains(name));

            if (!string.IsNullOrEmpty(department))
                query = query.Where(e => e.Department == department);

            if (!string.IsNullOrEmpty(state))
                query = query.Where(e => e.State == state);

            if (minSalary.HasValue)
                query = query.Where(e => e.Salary >= minSalary.Value);

            if (maxSalary.HasValue)
                query = query.Where(e => e.Salary <= maxSalary.Value);

            var employees = await query.ToListAsync();
            return Ok(employees);
        }

        // 🆕 Get Employees by Salary Range
        [HttpGet("salary-range")]
        public async Task<IActionResult> GetEmployeesBySalaryRange([FromQuery] decimal minSalary, [FromQuery] decimal maxSalary)
        {
            var employees = await _context.tblEmployees
                .Where(e => e.Salary >= minSalary && e.Salary <= maxSalary)
                .OrderBy(e => e.Salary)
                .ToListAsync();
            
            return Ok(employees);
        }

        // 🆕 Get Top Earners
        [HttpGet("top-earners")]
        public async Task<IActionResult> GetTopEarners([FromQuery] int count = 10)
        {
            var employees = await _context.tblEmployees
            .OrderByDescending(e => e.Salary)
            .Take(count)
            .ToListAsync();
             
            return Ok(employees);
        }

        // 🆕 Check if Email Exists
        [HttpGet("email-exists")]
        public async Task<IActionResult> CheckEmailExists([FromQuery] string email)
        {
            if (string.IsNullOrEmpty(email))
                return BadRequest("Email is required.");

            var exists = await _context.tblEmployees
                .AnyAsync(e => e.Email == email);
            
            return Ok(new { EmailExists = exists });
        }

        // 🆕 Get Employee Count by Department
        [HttpGet("count-by-department")]
        public async Task<IActionResult> GetEmployeeCountByDepartment()
        {
            var counts = await _context.tblEmployees
                .GroupBy(e => e.Department)
                .Select(g => new { Department = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .ToListAsync();
            
            return Ok(counts);
        }

        // 🆕 Export Employees to CSV (returns CSV data)
        [HttpGet("export-csv")]
        public async Task<IActionResult> ExportEmployeesToCsv()
        {
            var employees = await _context.tblEmployees.ToListAsync();
            
            var csv = "Id,EmployeeName,Email,Department,Salary,Address1,Address2,Address3,State,District,Pincode\n";
            foreach (var emp in employees)
            {
                csv += $"{emp.Id},{emp.EmployeeName},{emp.Email},{emp.Department},{emp.Salary},{emp.Address1},{emp.Address2},{emp.Address3},{emp.State},{emp.District},{emp.Pincode}\n";
            }

            return Content(csv, "text/csv");
        }
    }

    // 🆕 Request model for bulk operations
    public class BulkUpdateDepartmentRequest
    {
        public int[] EmployeeIds { get; set; } = Array.Empty<int>();
        public string NewDepartment { get; set; } = string.Empty;
    }
}
