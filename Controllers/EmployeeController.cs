using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Controllers
{
    public class EmployeeController : Controller
    {
        public IActionResult Index()
        {
            return View(); // This will load Views/Employee/Index.cshtml
        }

        public IActionResult List()
        {
            return View(); // This will load Views/Employee/List.cshtml
        }
    }
}
