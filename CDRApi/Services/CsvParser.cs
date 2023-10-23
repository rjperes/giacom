using CDRApi.Model;
using System.Globalization;

namespace CDRApi.Services
{
    public class CsvParser : ICsvParser
    {
        public char[] Separators { get; set; } = new char[] { ',' };

        private const int _numFields = 9;

        public async Task<IEnumerable<CallDto>> Parse(IFormFile file, CancellationToken cancellation = default)
        {
            ArgumentNullException.ThrowIfNull(file, nameof(file));

            if (!file.FileName.EndsWith(".csv", StringComparison.InvariantCultureIgnoreCase))
            {
                throw new ArgumentException("Passed file is not a CSV.", nameof(file));
            }

            using var reader = new StreamReader(file.OpenReadStream());
            var list = new LinkedList<CallDto>();

            while (true)
            {
                var line = await reader.ReadLineAsync(cancellation);

                if (string.IsNullOrWhiteSpace(line))
                {
                    break;
                }

                var parts = line.Split(Separators);

                if (parts.Length != _numFields)
                {
                    throw new InvalidOperationException("The file format is invalid.");
                }

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

            return list;
        }
    }
}
