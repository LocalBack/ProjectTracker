using ProjectTracker.Service.DTOs;

namespace ProjectTracker.Service.Services.Interfaces
{
    public interface IEmployeeService
    {
        Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync();
        Task<EmployeeDto> GetEmployeeByIdAsync(int id);
        Task<EmployeeDto> CreateEmployeeAsync(EmployeeDto employeeDto);
        Task<EmployeeDto> UpdateEmployeeAsync(int id, EmployeeDto employeeDto);
        Task<bool> DeleteEmployeeAsync(int id);
    }
}