
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using PerformanceEvaluation.Application.Mappings;
using PerformanceEvaluation.Infrastructure;
using PerformanceEvaluation.Infrastructure.Data;
using PerformanceEvaluation.Infrastructure.Seeding;
using Serilog;
using System.Text;
using System.Text.Json;

namespace PerformanceEvaluation.API;

public class Program
{
    public static async Task Main(string[] args)
    {
        // Configure Serilog
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File("logs/performance-evaluation-.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        try
        {
            Log.Information("Starting Performance Evaluation API");

            var builder = WebApplication.CreateBuilder(args);

            // Add Serilog
            builder.Host.UseSerilog();

            // Add services to the container
            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    // Configure camelCase JSON serialization for React frontend
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    options.JsonSerializerOptions.WriteIndented = true;
                });

            // Add Infrastructure services (EF Core, Repositories, Application Services)
            builder.Services.AddInfrastructure(builder.Configuration);

            // Add AutoMapper
            builder.Services.AddAutoMapper(typeof(MappingProfile));

            // Add FluentValidation
            builder.Services.AddValidatorsFromAssemblyContaining<MappingProfile>();

            // Add JWT Authentication
            var jwtSettings = builder.Configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"] ?? "YourSuperSecretKeyThatIsAtLeast32CharactersLong!";
            
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"] ?? "PerformanceEvaluationAPI",
                    ValidAudience = jwtSettings["Audience"] ?? "PerformanceEvaluationClient",
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                    ClockSkew = TimeSpan.Zero
                };
            });

            // Add Authorization
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("HR", policy => policy.RequireClaim("Role", "HR"));
                options.AddPolicy("Admin", policy => policy.RequireClaim("Role", "Admin"));
                options.AddPolicy("HROrAdmin", policy => policy.RequireClaim("Role", "HR", "Admin"));
                options.AddPolicy("Employee", policy => policy.RequireClaim("Role", "Employee", "HR", "Admin"));
            });

            // Add CORS - ALLOW ALL ORIGINS, ALL HEADERS, ALL METHODS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("ReactApp", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            // Add Swagger/OpenAPI
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new() { 
                    Title = "Performance Evaluation API", 
                    Version = "v1",
                    Description = "360-Degree Employee Performance Evaluation System API"
                });
                
                // Add JWT authentication to Swagger
                c.AddSecurityDefinition("Bearer", new()
                {
                    Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.",
                    Name = "Authorization",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new()
                {
                    {
                        new()
                        {
                            Reference = new() { Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme, Id = "Bearer" }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            var app = builder.Build();

            // Seed database
            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<PerformanceEvaluationDbContext>();
                await DataSeeder.SeedAsync(context);
            }

            // Configure the HTTP request pipeline
            
            // CORS MUST BE FIRST - BEFORE EVERYTHING ELSE!
            app.UseCors("ReactApp");
            
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Performance Evaluation API V1");
                    c.RoutePrefix = string.Empty; // Serve Swagger UI at root
                });
            }

            app.UseHttpsRedirection();

            // Use Authentication & Authorization
            app.UseAuthentication();
            app.UseAuthorization();

            // Global exception handling middleware
            app.UseMiddleware<GlobalExceptionMiddleware>();

            app.MapControllers();

            Log.Information("Performance Evaluation API started successfully");
            await app.RunAsync();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application terminated unexpectedly");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}

// Global Exception Handling Middleware
public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        
        var response = new
        {
            error = new
            {
                message = exception.Message,
                type = exception.GetType().Name
            }
        };

        context.Response.StatusCode = exception switch
        {
            ArgumentException => 400,
            UnauthorizedAccessException => 401,
            InvalidOperationException => 400,
            _ => 500
        };

        var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(jsonResponse);
    }
}
