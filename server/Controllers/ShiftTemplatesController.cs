using CondoAI.Server.Data;
using CondoAI.Server.Models;
using CondoAI.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CondoAI.Server.Controllers;

[ApiController]
[Route("admin/shift_templates")]
[Authorize(Policy = "AdminOnly")]
public class ShiftTemplatesController : ControllerBase
{
    private readonly AppDbContext _dbContext;
    private readonly AuditLogService _auditLog;

    public ShiftTemplatesController(AppDbContext dbContext, AuditLogService auditLog)
    {
        _dbContext = dbContext;
        _auditLog = auditLog;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var templates = await _dbContext.ShiftTemplates.AsNoTracking().ToListAsync(cancellationToken);
        return Ok(templates);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var template = await _dbContext.ShiftTemplates.FindAsync([id], cancellationToken);
        return template is null ? NotFound() : Ok(template);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ShiftTemplateRequest request, CancellationToken cancellationToken)
    {
        var template = new ShiftTemplate
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            IsActive = request.IsActive,
            CreatedAt = DateTimeOffset.UtcNow
        };

        _dbContext.ShiftTemplates.Add(template);
        await _dbContext.SaveChangesAsync(cancellationToken);
        await _auditLog.RecordAsync("shift_template.create", User.Identity?.Name ?? "system", template.Id.ToString(), cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = template.Id }, template);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] ShiftTemplateRequest request, CancellationToken cancellationToken)
    {
        var template = await _dbContext.ShiftTemplates.FindAsync([id], cancellationToken);
        if (template is null)
        {
            return NotFound();
        }

        template.Name = request.Name;
        template.StartTime = request.StartTime;
        template.EndTime = request.EndTime;
        template.IsActive = request.IsActive;

        await _dbContext.SaveChangesAsync(cancellationToken);
        await _auditLog.RecordAsync("shift_template.update", User.Identity?.Name ?? "system", template.Id.ToString(), cancellationToken);

        return Ok(template);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var template = await _dbContext.ShiftTemplates.FindAsync([id], cancellationToken);
        if (template is null)
        {
            return NotFound();
        }

        _dbContext.ShiftTemplates.Remove(template);
        await _dbContext.SaveChangesAsync(cancellationToken);
        await _auditLog.RecordAsync("shift_template.delete", User.Identity?.Name ?? "system", template.Id.ToString(), cancellationToken);

        return NoContent();
    }
}

public record ShiftTemplateRequest(string Name, TimeSpan StartTime, TimeSpan EndTime, bool IsActive);
