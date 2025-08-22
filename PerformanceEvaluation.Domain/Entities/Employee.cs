using System.ComponentModel.DataAnnotations;

namespace PerformanceEvaluation.Domain.Entities;

public class Employee
{
    public int Id { get; private set; }
    
    [Required, MaxLength(200)]
    public string FullName { get; private set; } = string.Empty;
    
    [Required, EmailAddress, MaxLength(200)]
    public string Email { get; private set; } = string.Empty;
    
    [Required, MaxLength(100)]
    public string Position { get; private set; } = string.Empty;
    
    [Required, MaxLength(100)]
    public string Department { get; private set; } = string.Empty;
    
    [Required, MaxLength(50)]
    public string Role { get; private set; } = string.Empty; // HR, Admin, Employee
    
    public DateTime CreatedAt { get; private set; }
    
    // Navigation properties
    private readonly List<EvaluationResult> _evaluationsGiven = new();
    private readonly List<EvaluationResult> _evaluationsReceived = new();
    private readonly List<AnonymousMapping> _anonymousMappings = new();
    
    public IReadOnlyCollection<EvaluationResult> EvaluationsGiven => _evaluationsGiven.AsReadOnly();
    public IReadOnlyCollection<EvaluationResult> EvaluationsReceived => _evaluationsReceived.AsReadOnly();
    public IReadOnlyCollection<AnonymousMapping> AnonymousMappings => _anonymousMappings.AsReadOnly();
    
    // Private constructor for EF Core
    private Employee() { }
    
    public Employee(string fullName, string email, string position, string department, string role)
    {
        FullName = fullName ?? throw new ArgumentNullException(nameof(fullName));
        Email = email ?? throw new ArgumentNullException(nameof(email));
        Position = position ?? throw new ArgumentNullException(nameof(position));
        Department = department ?? throw new ArgumentNullException(nameof(department));
        Role = role ?? throw new ArgumentNullException(nameof(role));
        CreatedAt = DateTime.UtcNow;
    }
    
    public void UpdateInfo(string fullName, string email, string position, string department, string role)
    {
        FullName = fullName ?? throw new ArgumentNullException(nameof(fullName));
        Email = email ?? throw new ArgumentNullException(nameof(email));
        Position = position ?? throw new ArgumentNullException(nameof(position));
        Department = department ?? throw new ArgumentNullException(nameof(department));
        Role = role ?? throw new ArgumentNullException(nameof(role));
    }
}
