using CondoAI.Server.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CondoAI.Server.Controllers;

[ApiController]
[Route("presence")]
[Authorize]
public class PresenceController : ControllerBase
{
    private readonly AppDbContext _dbContext;

    public PresenceController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet("today")]
    public async Task<IActionResult> GetToday(CancellationToken cancellationToken)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var records = await _dbContext.PresenceRecords
            .AsNoTracking()
            .Where(record => record.Date == today)
            .ToListAsync(cancellationToken);

        return Ok(records);
    }
}
