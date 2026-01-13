using CondoAI.Server.Data;
using CondoAI.Server.Models;
using CondoAI.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CondoAI.Server.Controllers;

[ApiController]
[Route("admin/employees")]
[Authorize(Policy = "AdminOnly")]
public class EmployeesController : ControllerBase
{
    private readonly AppDbContext _dbContext;
    private readonly AuditLogService _auditLog;

    public EmployeesController(AppDbContext dbContext, AuditLogService auditLog)
    {
        _dbContext = dbContext;
        _auditLog = auditLog;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var employees = await _dbContext.Employees.AsNoTracking().ToListAsync(cancellationToken);
        return Ok(employees);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var employee = await _dbContext.Employees.FindAsync([id], cancellationToken);
        return employee is null ? NotFound() : Ok(employee);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] EmployeeRequest request, CancellationToken cancellationToken)
    {
        var employee = new Employee
        {
            Id = Guid.NewGuid(),
            FullName = request.FullName,
            Role = request.Role,
            Department = request.Department,
            IsActive = request.IsActive,
            CreatedAt = DateTimeOffset.UtcNow
        };

        _dbContext.Employees.Add(employee);
        await _dbContext.SaveChangesAsync(cancellationToken);
        await _auditLog.RecordAsync("employee.create", User.Identity?.Name ?? "system", employee.Id.ToString(), cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = employee.Id }, employee);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] EmployeeRequest request, CancellationToken cancellationToken)
    {
        var employee = await _dbContext.Employees.FindAsync([id], cancellationToken);
        if (employee is null)
        {
            return NotFound();
        }

        employee.FullName = request.FullName;
        employee.Role = request.Role;
        employee.Department = request.Department;
        employee.IsActive = request.IsActive;

        await _dbContext.SaveChangesAsync(cancellationToken);
        await _auditLog.RecordAsync("employee.update", User.Identity?.Name ?? "system", employee.Id.ToString(), cancellationToken);

        return Ok(employee);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var employee = await _dbContext.Employees.FindAsync([id], cancellationToken);
        if (employee is null)
        {
            return NotFound();
        }

        _dbContext.Employees.Remove(employee);
        await _dbContext.SaveChangesAsync(cancellationToken);
        await _auditLog.RecordAsync("employee.delete", User.Identity?.Name ?? "system", employee.Id.ToString(), cancellationToken);

        return NoContent();
    }
}

public record EmployeeRequest(string FullName, string Role, string Department, bool IsActive);
