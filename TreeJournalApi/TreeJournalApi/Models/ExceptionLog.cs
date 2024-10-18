public class ExceptionLog
{
    public long Id { get; set; }
    public DateTime Timestamp { get; set; }
    public string EventId { get; set; }
    public string QueryParameters { get; set; }
    public string BodyParameters { get; set; }
    public string StackTrace { get; set; }
}
