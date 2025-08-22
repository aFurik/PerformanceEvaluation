using System.ComponentModel.DataAnnotations;

namespace PerformanceEvaluation.Application.DTOs;

public class UpdateEmployeeDto
{
    [Required, MaxLength(200)]
    public string FullName { get; set; } = string.Empty;
    
    [Required, EmailAddress, MaxLength(200)]
    public string Email { get; set; } = string.Empty;
    
    [Required, MaxLength(100)]
    public string Position { get; set; } = string.Empty;
    
    [Required, MaxLength(100)]
    public string Department { get; set; } = string.Empty;
    
    [Required, MaxLength(50)]
    public string Role { get; set; } = string.Empty; // HR, Admin, Employee
}
