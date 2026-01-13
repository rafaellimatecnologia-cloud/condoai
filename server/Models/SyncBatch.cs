namespace CondoAI.Server.Models;

public class SyncBatch
{
    public Guid Id { get; set; }
    public string ClientEventId { get; set; } = string.Empty;
    public string Payload { get; set; } = string.Empty;
    public DateTimeOffset ReceivedAt { get; set; } = DateTimeOffset.UtcNow;
}
