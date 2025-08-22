using Microsoft.EntityFrameworkCore;
using PerformanceEvaluation.Domain.Entities;
using PerformanceEvaluation.Infrastructure.Data;

namespace PerformanceEvaluation.Infrastructure.Seeding;

public static class DataSeeder
{
    public static async Task SeedAsync(PerformanceEvaluationDbContext context)
    {
        await context.Database.EnsureCreatedAsync();

        // Seed Competencies
        if (!await context.Competencies.AnyAsync())
        {
            var competencies = new List<Competency>
            {
                new("Communication", "Ability to communicate effectively with team members and stakeholders"),
                new("Leadership", "Demonstrates leadership qualities and ability to guide others"),
                new("Technical Skills", "Proficiency in technical aspects of the role"),
                new("Problem Solving", "Ability to identify and solve complex problems"),
                new("Teamwork", "Works effectively as part of a team"),
                new("Innovation", "Brings creative solutions and new ideas"),
                new("Time Management", "Manages time effectively and meets deadlines"),
                new("Adaptability", "Adapts well to change and new situations"),
                new("Customer Focus", "Demonstrates commitment to customer satisfaction"),
                new("Quality Orientation", "Maintains high standards of work quality")
            };

            await context.Competencies.AddRangeAsync(competencies);
            await context.SaveChangesAsync();
        }

        // Seed Employees
        if (!await context.Employees.AnyAsync())
        {
            var employees = new List<Employee>
            {
                // Demo Login Accounts
                new("Admin User", "admin@company.com", "System Administrator", "IT", "Admin"),
                new("HR Manager", "hr@company.com", "HR Manager", "Human Resources", "HR"),
                new("Demo Employee", "employee@company.com", "Software Developer", "IT", "Employee"),
                
                // HR Department
                new("Alice Johnson", "alice.johnson@company.com", "HR Manager", "Human Resources", "HR"),
                new("Bob Smith", "bob.smith@company.com", "HR Specialist", "Human Resources", "HR"),
                
                // Admin
                new("Carol Davis", "carol.davis@company.com", "System Administrator", "IT", "Admin"),
                
                // IT Department
                new("David Wilson", "david.wilson@company.com", "Senior Developer", "IT", "Employee"),
                new("Eva Brown", "eva.brown@company.com", "Frontend Developer", "IT", "Employee"),
                new("Frank Miller", "frank.miller@company.com", "Backend Developer", "IT", "Employee"),
                new("Grace Lee", "grace.lee@company.com", "DevOps Engineer", "IT", "Employee"),
                
                // Marketing Department
                new("Henry Taylor", "henry.taylor@company.com", "Marketing Manager", "Marketing", "Employee"),
                new("Ivy Chen", "ivy.chen@company.com", "Digital Marketing Specialist", "Marketing", "Employee"),
                new("Jack Anderson", "jack.anderson@company.com", "Content Creator", "Marketing", "Employee"),
                
                // Sales Department
                new("Karen White", "karen.white@company.com", "Sales Manager", "Sales", "Employee"),
                new("Liam Garcia", "liam.garcia@company.com", "Sales Representative", "Sales", "Employee"),
                new("Mia Rodriguez", "mia.rodriguez@company.com", "Account Executive", "Sales", "Employee"),
                
                // Finance Department
                new("Noah Martinez", "noah.martinez@company.com", "Finance Manager", "Finance", "Employee"),
                new("Olivia Thompson", "olivia.thompson@company.com", "Financial Analyst", "Finance", "Employee")
            };

            await context.Employees.AddRangeAsync(employees);
            await context.SaveChangesAsync();
        }

        // Seed Sample Evaluation Session
        if (!await context.EvaluationSessions.AnyAsync())
        {
            var hrManager = await context.Employees.FirstAsync(e => e.Role == "HR");
            
            var evaluationSessions = new List<EvaluationSession>
            {
                new("Q4 2024 Performance Review", 
                    new DateTime(2024, 10, 1), 
                    new DateTime(2024, 12, 31), 
                    hrManager.Id),
                new("Mid-Year 2024 Review", 
                    new DateTime(2024, 6, 1), 
                    new DateTime(2024, 8, 31), 
                    hrManager.Id)
            };

            await context.EvaluationSessions.AddRangeAsync(evaluationSessions);
            await context.SaveChangesAsync();
        }

        // Seed Anonymous Mappings for active session
        var activeSession = await context.EvaluationSessions
            .FirstOrDefaultAsync(es => es.StartDate <= DateTime.UtcNow && es.EndDate >= DateTime.UtcNow);
            
        if (activeSession != null && !await context.AnonymousMappings.AnyAsync(am => am.SessionId == activeSession.Id))
        {
            var employees = await context.Employees.Where(e => e.Role == "Employee").ToListAsync();
            var anonymousMappings = new List<AnonymousMapping>();

            foreach (var employee in employees)
            {
                anonymousMappings.Add(new AnonymousMapping(activeSession.Id, employee.Id));
            }

            await context.AnonymousMappings.AddRangeAsync(anonymousMappings);
            await context.SaveChangesAsync();
        }

        // Seed Sample Evaluations (for demonstration)
        if (!await context.EvaluationResults.AnyAsync() && activeSession != null)
        {
            var employees = await context.Employees.Where(e => e.Role == "Employee").ToListAsync();
            var competencies = await context.Competencies.ToListAsync();
            var evaluations = new List<EvaluationResult>();

            // Create some sample evaluations (limited to avoid too much data)
            var random = new Random();
            var evaluatorCount = Math.Min(3, employees.Count);
            var evaluatedCount = Math.Min(2, employees.Count);

            for (int i = 0; i < evaluatorCount; i++)
            {
                for (int j = 0; j < evaluatedCount; j++)
                {
                    if (i != j) // No self-evaluation
                    {
                        foreach (var competency in competencies.Take(3)) // Only first 3 competencies
                        {
                            var score = random.Next(3, 6); // Random score between 3-5
                            var comments = new[]
                            {
                                "Shows great potential in this area.",
                                "Consistently delivers quality work.",
                                "Could benefit from additional training.",
                                "Excellent collaboration skills.",
                                "Demonstrates strong problem-solving abilities."
                            };

                            evaluations.Add(new EvaluationResult(
                                activeSession.Id,
                                employees[j].Id,
                                employees[i].Id,
                                competency.Id,
                                score,
                                comments[random.Next(comments.Length)]
                            ));
                        }
                    }
                }
            }

            await context.EvaluationResults.AddRangeAsync(evaluations);
            await context.SaveChangesAsync();
        }
    }
}
