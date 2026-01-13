using CondoAI.Server.Data;
using CondoAI.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CondoAI.Server.Controllers;

[ApiController]
[Route("sync")]
[Authorize]
public class SyncController : ControllerBase
{
    private readonly AppDbContext _dbContext;

    public SyncController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpPost("batch")]
    public async Task<IActionResult> Batch([FromBody] SyncBatchRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.ClientEventId))
        {
            return BadRequest(new { error = "clientEventId required" });
        }

        var existing = await _dbContext.SyncBatches
            .AsNoTracking()
            .FirstOrDefaultAsync(batch => batch.ClientEventId == request.ClientEventId, cancellationToken);

        if (existing is not null)
        {
            return Ok(new { status = "duplicate", batchId = existing.Id });
        }

        var batch = new SyncBatch
        {
            Id = Guid.NewGuid(),
            ClientEventId = request.ClientEventId,
            Payload = request.Payload ?? string.Empty,
            ReceivedAt = DateTimeOffset.UtcNow
        };

        _dbContext.SyncBatches.Add(batch);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Ok(new { status = "accepted", batchId = batch.Id });
    }
}

public record SyncBatchRequest(string ClientEventId, string? Payload);
