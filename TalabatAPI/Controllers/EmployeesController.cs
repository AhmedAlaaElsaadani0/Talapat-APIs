using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.API.Errors;
using Talabat.Core.Entities;
using Talabat.Core.IRepositories;
using Talabat.Core.Specifications.Employees;

namespace Talabat.API.Controllers
{
    public class EmployeesController : BaseAPIController
    {
        private readonly IGenericRepository<Employee> employeeRepository;

        public EmployeesController(IGenericRepository<Employee> employeeRepository_)
        {
            employeeRepository = employeeRepository_;
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<Employee>>> GetEmployees()
        {
            var spec = new EmployeeWithDepartmentSpecifications();

            var employees = await employeeRepository.GetAllWithSpecAsync(spec);

            return Ok(employees);
        }

        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [HttpGet("{id}")]
        public async Task<ActionResult<IReadOnlyList<Employee>>> GetProducts(int id)
        {
            var spec = new EmployeeWithDepartmentSpecifications(id);

            var employee = await employeeRepository.GetByIdWithSpecAsync(spec);

            if (employee is null)
                return NotFound(new ApiResponse(404));

            return Ok(employee);
        }
    }
}
