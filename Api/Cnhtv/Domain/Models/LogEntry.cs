public sealed class LogEntry
{
    public Guid Id { get; set; }
    public DateTime Timestamp { get; set; }
    public int EventId { get; set; }
    public string Level { get; set; } = string.Empty;
    public string Logger { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string CallSite { get; set; } = string.Empty;
    public string? Exception { get; set; }
    public string Source { get; set; } = string.Empty;

    public LogEntry() { }
}
