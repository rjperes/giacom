using CDRApi.Model;

namespace CDRApi.Services
{
    public interface ICsvParser
    {
        char[] Separators { get; set; }
        bool IgnoreInvalidLines { get; set; }

        Task<IEnumerable<CallDto>> Parse(IFormFile file, CancellationToken cancellation = default);
    }
}
