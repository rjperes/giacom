using CDRModel;

namespace CDRServices
{
    public class CallStats
    {
        public int TotalCount { get; set; }
        public int TotalDuration { get; set; }
    }

    public class CallerStats
    {
        public string? Recipient { get; set; }
        public DateTime CallDate { get; set; }
        public TimeSpan EndTime { get; set; }
        public int Duration { get; set; }
        public decimal Cost { get; set; }
        public string? Reference { get; set; }
        public string? Currency { get; set; }
        public CallType Type { get; set; }
    }

    public interface ICDRRepository
    {
        Task<int> Save(IEnumerable<Call> calls, CancellationToken cancellation = default);
        Task<Call> Find(string reference, CancellationToken cancellation = default);
        Task<CallStats> CallStats(DateTime from, DateTime to, CallType? type = null, CancellationToken cancellation = default);
        Task<IEnumerable<CallerStats>> CallerStats(string callerId, DateTime from, DateTime to, CallType? type = null, CancellationToken cancellation = default);
    }
}