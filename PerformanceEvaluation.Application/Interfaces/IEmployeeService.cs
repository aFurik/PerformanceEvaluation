using PerformanceEvaluation.Application.DTOs;

namespace PerformanceEvaluation.Application.Interfaces;

public interface IEmployeeService
{
    Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync();
    Task<EmployeeDto?> GetEmployeeByIdAsync(int id);
    Task<EmployeeDto?> GetEmployeeByEmailAsync(string email);
    Task<EmployeeDto> CreateEmployeeAsync(CreateEmployeeDto createEmployeeDto);
    Task<EmployeeDto?> UpdateEmployeeAsync(int id, UpdateEmployeeDto updateEmployeeDto);
    Task<bool> DeleteEmployeeAsync(int id);
    Task<IEnumerable<EmployeeDto>> GetEmployeesByDepartmentAsync(string department);
    Task<IEnumerable<EmployeeDto>> GetEmployeesByRoleAsync(string role);
    Task<bool> EmployeeExistsAsync(int id);
}
