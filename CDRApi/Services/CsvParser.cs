using CDRApi.Model;
using System.Globalization;

namespace CDRApi.Services
{
    public class CsvParser : ICsvParser
    {
        private const int _numFields = 9;
        private readonly ILogger<CsvParser> _logger;

        public CsvParser(ILogger<CsvParser> logger)
        {
            _logger = logger;
        }

        public char[] Separators { get; set; } = new char[] { ',' };
        public bool IgnoreInvalidLines { get; set; }

        public async Task<IEnumerable<CallDto>> Parse(IFormFile file, CancellationToken cancellation = default)
        {
            ArgumentNullException.ThrowIfNull(file, nameof(file));

            if (!file.FileName.EndsWith(".csv", StringComparison.InvariantCultureIgnoreCase))
            {
                throw new ArgumentException("Passed file is not a CSV.", nameof(file));
            }

            using var reader = new StreamReader(file.OpenReadStream());
            var list = new LinkedList<CallDto>();

            var line = await reader.ReadLineAsync(cancellation);

            if (string.IsNullOrWhiteSpace(line))
            {
                throw new InvalidOperationException("The file format is invalid - missing header.");
            }

            var parts = line.Split(Separators);

            if (parts.Length != _numFields)
            {
                throw new InvalidOperationException("The file format is invalid - incorrect number of fields.");
            }

            while (true)
            {
                line = await reader.ReadLineAsync(cancellation);

                if (string.IsNullOrWhiteSpace(line))
                {
                    break;
                }

                parts = line.Split(Separators);

                if (parts.Length != _numFields)
                {
                    throw new InvalidOperationException("The file format is invalid - incorrect number of fields.");
                }

                try
                {
                    list.AddLast(new CallDto
                    {
                        Caller_Id = parts[0],
                        Recipient = parts[1],
                        Call_Date = parts[2],
                        End_Time = parts[3],
                        Duration = int.Parse(parts[4], CultureInfo.InvariantCulture),
                        Cost = decimal.Parse(parts[5], CultureInfo.InvariantCulture),
                        Reference = parts[6],
                        Currency = parts[7],
                        Type = int.Parse(parts[8], CultureInfo.InvariantCulture)
                    });
                }
                catch
                {
                    if (!this.IgnoreInvalidLines)
                    {
                        throw;
                    }
                    _logger.LogError($"Invalid field detected - skipping");
                }
            }

            return list;
        }
    }
}
