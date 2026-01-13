using CondoAI.Server.Data;
using CondoAI.Server.Models;

namespace CondoAI.Server.Services;

public class AuditLogService
{
    private readonly AppDbContext _dbContext;

    public AuditLogService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task RecordAsync(string action, string actor, string? details = null, CancellationToken cancellationToken = default)
    {
        var log = new AuditLog
        {
            Id = Guid.NewGuid(),
            Action = action,
            Actor = actor,
            Details = details,
            CreatedAt = DateTimeOffset.UtcNow
        };

        _dbContext.AuditLogs.Add(log);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
