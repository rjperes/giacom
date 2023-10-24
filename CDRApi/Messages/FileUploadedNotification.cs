using CDRModel;
using MediatR;

namespace CDRApi.Messages
{
    public class FileUploadedNotification : INotification
    {
        public FileUploadedNotification(IEnumerable<Call> calls)
        {
            this.Calls = calls;
        }

        public IEnumerable<Call> Calls { get; }
    }
}
