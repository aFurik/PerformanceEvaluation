using AutoMapper;
using PerformanceEvaluation.Application.DTOs;
using PerformanceEvaluation.Application.Interfaces;
using PerformanceEvaluation.Domain.Entities;

namespace PerformanceEvaluation.Application.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IMapper _mapper;

    public EmployeeService(IEmployeeRepository employeeRepository, IMapper mapper)
    {
        _employeeRepository = employeeRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync()
    {
        var employees = await _employeeRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<EmployeeDto>>(employees);
    }

    public async Task<EmployeeDto?> GetEmployeeByIdAsync(int id)
    {
        var employee = await _employeeRepository.GetByIdAsync(id);
        return employee != null ? _mapper.Map<EmployeeDto>(employee) : null;
    }

    public async Task<EmployeeDto?> GetEmployeeByEmailAsync(string email)
    {
        var employee = await _employeeRepository.GetByEmailAsync(email);
        return employee != null ? _mapper.Map<EmployeeDto>(employee) : null;
    }

    public async Task<EmployeeDto> CreateEmployeeAsync(CreateEmployeeDto createEmployeeDto)
    {
        // Check if email already exists
        var existingEmployee = await _employeeRepository.GetByEmailAsync(createEmployeeDto.Email);
        if (existingEmployee != null)
        {
            throw new InvalidOperationException($"Employee with email {createEmployeeDto.Email} already exists.");
        }

        var employee = new Employee(
            createEmployeeDto.FullName,
            createEmployeeDto.Email,
            createEmployeeDto.Position,
            createEmployeeDto.Department,
            createEmployeeDto.Role
        );

        await _employeeRepository.AddAsync(employee);
        await _employeeRepository.SaveChangesAsync();

        return _mapper.Map<EmployeeDto>(employee);
    }

    public async Task<EmployeeDto?> UpdateEmployeeAsync(int id, UpdateEmployeeDto updateEmployeeDto)
    {
        var employee = await _employeeRepository.GetByIdAsync(id);
        if (employee == null)
        {
            return null;
        }

        // Check if email is being changed and if new email already exists
        if (employee.Email != updateEmployeeDto.Email)
        {
            var existingEmployee = await _employeeRepository.GetByEmailAsync(updateEmployeeDto.Email);
            if (existingEmployee != null)
            {
                throw new InvalidOperationException($"Employee with email {updateEmployeeDto.Email} already exists.");
            }
        }

        employee.UpdateInfo(
            updateEmployeeDto.FullName,
            updateEmployeeDto.Email,
            updateEmployeeDto.Position,
            updateEmployeeDto.Department,
            updateEmployeeDto.Role
        );

        await _employeeRepository.UpdateAsync(employee);
        await _employeeRepository.SaveChangesAsync();

        return _mapper.Map<EmployeeDto>(employee);
    }

    public async Task<bool> DeleteEmployeeAsync(int id)
    {
        var employee = await _employeeRepository.GetByIdAsync(id);
        if (employee == null)
        {
            return false;
        }

        await _employeeRepository.DeleteAsync(employee);
        await _employeeRepository.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<EmployeeDto>> GetEmployeesByDepartmentAsync(string department)
    {
        var employees = await _employeeRepository.GetByDepartmentAsync(department);
        return _mapper.Map<IEnumerable<EmployeeDto>>(employees);
    }

    public async Task<IEnumerable<EmployeeDto>> GetEmployeesByRoleAsync(string role)
    {
        var employees = await _employeeRepository.GetByRoleAsync(role);
        return _mapper.Map<IEnumerable<EmployeeDto>>(employees);
    }

    public async Task<bool> EmployeeExistsAsync(int id)
    {
        return await _employeeRepository.ExistsAsync(id);
    }
}

// Repository interface that will be implemented in Infrastructure layer
public interface IEmployeeRepository
{
    Task<IEnumerable<Employee>> GetAllAsync();
    Task<Employee?> GetByIdAsync(int id);
    Task<Employee?> GetByEmailAsync(string email);
    Task<IEnumerable<Employee>> GetByDepartmentAsync(string department);
    Task<IEnumerable<Employee>> GetByRoleAsync(string role);
    Task<Employee> AddAsync(Employee employee);
    Task<Employee> UpdateAsync(Employee employee);
    Task DeleteAsync(Employee employee);
    Task<bool> ExistsAsync(int id);
    Task SaveChangesAsync();
}
