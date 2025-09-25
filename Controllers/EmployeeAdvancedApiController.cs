using EmployeeManagement.Models;
using EmployeeManagement.Services;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Controllers
{
    [ApiController]
    [Route("api/v2/[controller]")]
    public class EmployeeAdvancedApiController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeAdvancedApiController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        // Get Employee by ID with detailed response
        [HttpGet("get/{id}")]
        public async Task<IActionResult> GetEmployee(int id)
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(id);
            if (employee == null)
                return NotFound(new { Message = "Employee not found", EmployeeId = id });

            return Ok(new { Success = true, Data = employee });
        }

        // Get all employees with optional filtering
        [HttpGet("list")]
        public async Task<IActionResult> GetEmployees([FromQuery] string? department, [FromQuery] string? state)
        {
            IEnumerable<Employee> employees;

            if (!string.IsNullOrEmpty(department))
                employees = await _employeeService.GetEmployeesByDepartmentAsync(department);
            else if (!string.IsNullOrEmpty(state))
                employees = await _employeeService.GetEmployeesByStateAsync(state);
            else
                employees = await _employeeService.GetAllEmployeesAsync();

            return Ok(new { Success = true, Count = employees.Count(), Data = employees });
        }

        // Get employees with pagination
        [HttpGet("paginated")]
        public async Task<IActionResult> GetEmployeesPaginated([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _employeeService.GetEmployeesPaginatedAsync(page, pageSize);
            return Ok(new { Success = true, Data = result });
        }

        // Create employee with validation
        [HttpPost("create")]
        public async Task<IActionResult> CreateEmployee([FromBody] EmployeeCreateDto employeeDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { Success = false, Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) });

            // Check if email already exists
            if (await _employeeService.CheckEmailExistsAsync(employeeDto.Email))
                return Conflict(new { Success = false, Message = "Email already exists" });

            var employee = await _employeeService.CreateEmployeeAsync(employeeDto);
            return CreatedAtAction(nameof(GetEmployee), new { id = employee.Id }, 
                new { Success = true, Message = "Employee created successfully", Data = employee });
        }

        // Update employee with validation
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateEmployee(int id, [FromBody] EmployeeUpdateDto employeeDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { Success = false, Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) });

            var employee = await _employeeService.UpdateEmployeeAsync(id, employeeDto);
            if (employee == null)
                return NotFound(new { Success = false, Message = "Employee not found" });

            return Ok(new { Success = true, Message = "Employee updated successfully", Data = employee });
        }

        // Delete employee
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var success = await _employeeService.DeleteEmployeeAsync(id);
            if (!success)
                return NotFound(new { Success = false, Message = "Employee not found" });

            return Ok(new { Success = true, Message = "Employee deleted successfully" });
        }

        // Search employees by name
        [HttpGet("search")]
        public async Task<IActionResult> SearchEmployees([FromQuery] string name)
        {
            if (string.IsNullOrEmpty(name))
                return BadRequest(new { Success = false, Message = "Search name is required" });

            var employees = await _employeeService.SearchEmployeesAsync(name);
            return Ok(new { Success = true, Count = employees.Count(), Data = employees });
        }

        // Advanced search with multiple filters
        [HttpPost("advanced-search")]
        public async Task<IActionResult> AdvancedSearch([FromBody] EmployeeSearchDto searchDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { Success = false, Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) });

            var employees = await _employeeService.AdvancedSearchAsync(searchDto);
            return Ok(new { Success = true, Count = employees.Count(), Data = employees });
        }

        // Advanced search with pagination
        [HttpPost("advanced-search-paginated")]
        public async Task<IActionResult> AdvancedSearchPaginated([FromBody] EmployeeSearchDto searchDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { Success = false, Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) });

            var result = await _employeeService.AdvancedSearchPaginatedAsync(searchDto);
            return Ok(new { Success = true, Data = result });
        }

        // Get employees by salary range
        [HttpGet("salary-range")]
        public async Task<IActionResult> GetEmployeesBySalaryRange([FromQuery] decimal minSalary, [FromQuery] decimal maxSalary)
        {
            if (minSalary > maxSalary)
                return BadRequest(new { Success = false, Message = "Min salary cannot be greater than max salary" });

            var employees = await _employeeService.GetEmployeesBySalaryRangeAsync(minSalary, maxSalary);
            return Ok(new { Success = true, Count = employees.Count(), Data = employees });
        }

        // Get top earners
        [HttpGet("top-earners")]
        public async Task<IActionResult> GetTopEarners([FromQuery] int count = 10)
        {
            if (count < 1 || count > 100)
                return BadRequest(new { Success = false, Message = "Count must be between 1 and 100" });

            var employees = await _employeeService.GetTopEarnersAsync(count);
            return Ok(new { Success = true, Count = employees.Count(), Data = employees });
        }

        // Get employee statistics
        [HttpGet("statistics")]
        public async Task<IActionResult> GetEmployeeStatistics()
        {
            var statistics = await _employeeService.GetEmployeeStatisticsAsync();
            return Ok(new { Success = true, Data = statistics });
        }

        // Get all departments
        [HttpGet("departments")]
        public async Task<IActionResult> GetDepartments()
        {
            var departments = await _employeeService.GetDepartmentsAsync();
            return Ok(new { Success = true, Count = departments.Count(), Data = departments });
        }

        // Get all states
        [HttpGet("states")]
        public async Task<IActionResult> GetStates()
        {
            var states = await _employeeService.GetStatesAsync();
            return Ok(new { Success = true, Count = states.Count(), Data = states });
        }

        // Check if email exists
        [HttpGet("email-exists")]
        public async Task<IActionResult> CheckEmailExists([FromQuery] string email)
        {
            if (string.IsNullOrEmpty(email))
                return BadRequest(new { Success = false, Message = "Email is required" });

            var exists = await _employeeService.CheckEmailExistsAsync(email);
            return Ok(new { Success = true, Data = new { EmailExists = exists } });
        }

        // Bulk delete employees
        [HttpDelete("bulk-delete")]
        public async Task<IActionResult> BulkDeleteEmployees([FromBody] int[] employeeIds)
        {
            if (employeeIds == null || employeeIds.Length == 0)
                return BadRequest(new { Success = false, Message = "Employee IDs are required" });

            var success = await _employeeService.BulkDeleteEmployeesAsync(employeeIds);
            if (!success)
                return NotFound(new { Success = false, Message = "No employees found with the provided IDs" });

            return Ok(new { Success = true, Message = $"Successfully deleted {employeeIds.Length} employees" });
        }

        // Bulk update department
        [HttpPut("bulk-update-department")]
        public async Task<IActionResult> BulkUpdateDepartment([FromBody] BulkUpdateDepartmentRequest request)
        {
            if (request.EmployeeIds == null || request.EmployeeIds.Length == 0)
                return BadRequest(new { Success = false, Message = "Employee IDs are required" });

            if (string.IsNullOrEmpty(request.NewDepartment))
                return BadRequest(new { Success = false, Message = "New department is required" });

            var success = await _employeeService.BulkUpdateDepartmentAsync(request.EmployeeIds, request.NewDepartment);
            if (!success)
                return NotFound(new { Success = false, Message = "No employees found with the provided IDs" });

            return Ok(new { Success = true, Message = $"Successfully updated department for {request.EmployeeIds.Length} employees" });
        }

        // Export employees to CSV
        [HttpGet("export-csv")]
        public async Task<IActionResult> ExportToCsv()
        {
            var csv = await _employeeService.ExportToCsvAsync();
            return Content(csv, "text/csv", System.Text.Encoding.UTF8);
        }

        // Get employee count by department
        [HttpGet("count-by-department")]
        public async Task<IActionResult> GetEmployeeCountByDepartment()
        {
            var statistics = await _employeeService.GetEmployeeStatisticsAsync();
            var departmentCounts = statistics.DepartmentBreakdown;
            return Ok(new { Success = true, Count = departmentCounts.Count, Data = departmentCounts });
        }

        // Health check endpoint
        [HttpGet("health")]
        public IActionResult HealthCheck()
        {
            return Ok(new { Success = true, Message = "Employee Advanced API is running", Timestamp = DateTime.UtcNow });
        }
    }
}
