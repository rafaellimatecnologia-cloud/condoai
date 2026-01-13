using CondoAI.Server.Data;
using CondoAI.Server.Models;
using CondoAI.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CondoAI.Server.Controllers;

[ApiController]
[Route("admin/config")]
[Authorize(Policy = "AdminOnly")]
public class ConfigController : ControllerBase
{
    private readonly AppDbContext _dbContext;
    private readonly AuditLogService _auditLog;

    public ConfigController(AppDbContext dbContext, AuditLogService auditLog)
    {
        _dbContext = dbContext;
        _auditLog = auditLog;
    }

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var configs = await _dbContext.SystemConfigs.AsNoTracking().ToListAsync(cancellationToken);

        var response = new ConfigResponse(
            configs.FirstOrDefault(c => c.Key == "qr")?.Value,
            configs.FirstOrDefault(c => c.Key == "network")?.Value,
            configs.FirstOrDefault(c => c.Key == "geofence")?.Value);

        return Ok(response);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] ConfigRequest request, CancellationToken cancellationToken)
    {
        await UpsertAsync("qr", request.Qr, cancellationToken);
        await UpsertAsync("network", request.Network, cancellationToken);
        await UpsertAsync("geofence", request.Geofence, cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);
        await _auditLog.RecordAsync("config.update", User.Identity?.Name ?? "system", "admin/config", cancellationToken);

        return Ok(request);
    }

    private async Task UpsertAsync(string key, string? value, CancellationToken cancellationToken)
    {
        var entry = await _dbContext.SystemConfigs.FirstOrDefaultAsync(config => config.Key == key, cancellationToken);
        if (entry is null)
        {
            if (value is null)
            {
                return;
            }

            _dbContext.SystemConfigs.Add(new SystemConfig
            {
                Id = Guid.NewGuid(),
                Key = key,
                Value = value,
                UpdatedAt = DateTimeOffset.UtcNow
            });
            return;
        }

        if (value is null)
        {
            _dbContext.SystemConfigs.Remove(entry);
            return;
        }

        entry.Value = value;
        entry.UpdatedAt = DateTimeOffset.UtcNow;
    }
}

public record ConfigRequest(string? Qr, string? Network, string? Geofence);

public record ConfigResponse(string? Qr, string? Network, string? Geofence);
