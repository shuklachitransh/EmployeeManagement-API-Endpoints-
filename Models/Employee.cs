using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Models
{
    public class Employee
    {
        public int Id { get; set; }  
        
        [Required(ErrorMessage = "Employee name is required")]
        [StringLength(100, ErrorMessage = "Employee name cannot exceed 100 characters")]
        public string EmployeeName { get; set; } = string.Empty;  
        
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;         
        
        [Required(ErrorMessage = "Department is required")]
        [StringLength(50, ErrorMessage = "Department cannot exceed 50 characters")]
        public string Department { get; set; } = string.Empty;    
        
        [Range(0, double.MaxValue, ErrorMessage = "Salary must be a positive number")]
        public decimal Salary { get; set; }                       

        [StringLength(200, ErrorMessage = "Address1 cannot exceed 200 characters")]
        public string Address1 { get; set; } = string.Empty;
        
        [StringLength(200, ErrorMessage = "Address2 cannot exceed 200 characters")]
        public string Address2 { get; set; } = string.Empty;
        
        [StringLength(200, ErrorMessage = "Address3 cannot exceed 200 characters")]
        public string Address3 { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "State is required")]
        [StringLength(50, ErrorMessage = "State cannot exceed 50 characters")]
        public string State { get; set; } = string.Empty;
        
        [StringLength(50, ErrorMessage = "District cannot exceed 50 characters")]
        public string District { get; set; } = string.Empty;
        
        [StringLength(10, ErrorMessage = "Pincode cannot exceed 10 characters")]
        public string Pincode { get; set; } = string.Empty;
    }

    // DTOs for API operations
    public class EmployeeCreateDto
    {
        [Required(ErrorMessage = "Employee name is required")]
        [StringLength(100, ErrorMessage = "Employee name cannot exceed 100 characters")]
        public string EmployeeName { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Department is required")]
        [StringLength(50, ErrorMessage = "Department cannot exceed 50 characters")]
        public string Department { get; set; } = string.Empty;
        
        [Range(0, double.MaxValue, ErrorMessage = "Salary must be a positive number")]
        public decimal Salary { get; set; }
        
        [StringLength(200, ErrorMessage = "Address1 cannot exceed 200 characters")]
        public string Address1 { get; set; } = string.Empty;
        
        [StringLength(200, ErrorMessage = "Address2 cannot exceed 200 characters")]
        public string Address2 { get; set; } = string.Empty;
        
        [StringLength(200, ErrorMessage = "Address3 cannot exceed 200 characters")]
        public string Address3 { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "State is required")]
        [StringLength(50, ErrorMessage = "State cannot exceed 50 characters")]
        public string State { get; set; } = string.Empty;
        
        [StringLength(50, ErrorMessage = "District cannot exceed 50 characters")]
        public string District { get; set; } = string.Empty;
        
        [StringLength(10, ErrorMessage = "Pincode cannot exceed 10 characters")]
        public string Pincode { get; set; } = string.Empty;
    }

    public class EmployeeUpdateDto
    {
        [Required(ErrorMessage = "Employee name is required")]
        [StringLength(100, ErrorMessage = "Employee name cannot exceed 100 characters")]
        public string EmployeeName { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Department is required")]
        [StringLength(50, ErrorMessage = "Department cannot exceed 50 characters")]
        public string Department { get; set; } = string.Empty;
        
        [Range(0, double.MaxValue, ErrorMessage = "Salary must be a positive number")]
        public decimal Salary { get; set; }
        
        [StringLength(200, ErrorMessage = "Address1 cannot exceed 200 characters")]
        public string Address1 { get; set; } = string.Empty;
        
        [StringLength(200, ErrorMessage = "Address2 cannot exceed 200 characters")]
        public string Address2 { get; set; } = string.Empty;
        
        [StringLength(200, ErrorMessage = "Address3 cannot exceed 200 characters")]
        public string Address3 { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "State is required")]
        [StringLength(50, ErrorMessage = "State cannot exceed 50 characters")]
        public string State { get; set; } = string.Empty;
        
        [StringLength(50, ErrorMessage = "District cannot exceed 50 characters")]
        public string District { get; set; } = string.Empty;
        
        [StringLength(10, ErrorMessage = "Pincode cannot exceed 10 characters")]
        public string Pincode { get; set; } = string.Empty;
    }

    public class EmployeeSearchDto
    {
        public string? Name { get; set; }
        public string? Department { get; set; }
        public string? State { get; set; }
        public decimal? MinSalary { get; set; }
        public decimal? MaxSalary { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class PaginatedResult<T>
    {
        public List<T> Data { get; set; } = new List<T>();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }

    public class EmployeeStatisticsDto
    {
        public int TotalEmployees { get; set; }
        public decimal AverageSalary { get; set; }
        public decimal MaxSalary { get; set; }
        public decimal MinSalary { get; set; }
        public List<DepartmentCount> DepartmentBreakdown { get; set; } = new List<DepartmentCount>();
        public List<StateCount> StateBreakdown { get; set; } = new List<StateCount>();
    }

    public class DepartmentCount
    {
        public string Department { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public class StateCount
    {
        public string State { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}
