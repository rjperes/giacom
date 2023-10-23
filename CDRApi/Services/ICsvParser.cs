using CDRApi.Model;

namespace CDRApi.Services
{
    public interface ICsvParser
    {
        Task<IEnumerable<CallDto>> Parse(IFormFile file, CancellationToken cancellation = default);
    }
}
