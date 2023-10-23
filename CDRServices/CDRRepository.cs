using CDRModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CDRServices
{
    public class CDRRepository : ICDRRepository
    {
        private readonly CDRContext _context;
        private readonly ILogger<CDRRepository> _logger;

        public CDRRepository(CDRContext context, ILogger<CDRRepository> logger)
        {
            ArgumentNullException.ThrowIfNull(context, nameof(context));
            ArgumentNullException.ThrowIfNull(logger, nameof(logger));

            _context = context;
            _logger = logger;
        }

        public async Task<Call> Find(string reference, CancellationToken cancellation = default)
        {
            ArgumentException.ThrowIfNullOrEmpty(reference, nameof(reference));

            try
            {
                var call = await _context!.Calls!.Where(x => x.Reference == reference).SingleOrDefaultAsync(cancellation);

                return call!;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while trying to find call with reference {reference}");
                throw;
            }
        }

        public async Task<int> Save(IEnumerable<Call> calls, CancellationToken cancellation = default)
        {
            ArgumentNullException.ThrowIfNull(calls, nameof(calls));

            try
            {
                foreach (var call in calls)
                {
                    await _context!.Calls!.AddAsync(call, cancellation);
                }

                var savedCalls = await _context.SaveChangesAsync(cancellation);

                _logger.LogDebug($"{savedCalls} saved");

                return savedCalls;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while trying to save calls");
                throw;
            }
        }

        public async Task<CallStats> CallStats(DateTime from, DateTime to, CallType? type = null, CancellationToken cancellation = default)
        {
            try
            {
                if (from > to)
                {
                    throw new ArgumentException("Invalid date range.", nameof(to));
                }

                var query = _context!.Calls!.Where(x => x.CallDate >= from && x.CallDate <= to);

                if (type != null)
                {
                    query = query.Where(x => x.Type == type);
                }

                var stats = await query.GroupBy(x => x.Duration).Select(x => new CallStats { TotalCount = x.Count(), TotalDuration = x.Sum(y => y.Duration) }).SingleOrDefaultAsync(cancellation);

                return stats!;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while trying to retrieve call statistics");
                throw;
            }
        }

        public async Task<IEnumerable<CallerStats>> CallerStats(string callerId, DateTime from, DateTime to, CallType? type = null, CancellationToken cancellation = default)
        {
            ArgumentException.ThrowIfNullOrEmpty(callerId, nameof(callerId));

            if (from > to)
            {
                throw new ArgumentException("Invalid date range.", nameof(to));
            }

            try
            {
                var query = _context!.Calls!.Where(x => x.CallerId == callerId && x.CallDate >= from && x.CallDate <= to);

                if (type != null)
                {
                    query = query.Where(x => x.Type == type);
                }

                var calls = await query.ToListAsync(cancellation);

                return calls.Select(x => new CallerStats
                {
                    CallDate = x.CallDate,
                    Cost = x.Cost,
                    Currency = x.Currency,
                    Duration = x.Duration,
                    EndTime = x.EndTime,
                    Recipient = x.Recipient,
                    Reference = x.Reference,
                    Type = x.Type
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while trying to retrieve caller statistics");
                throw;
            }
        }
    }
}