using CDRModel;
using MediatR;

namespace CDRApi.Messages
{
    public class FileUploadedCommand : ICommand, INotification
    {
        public FileUploadedCommand(IEnumerable<Call> calls)
        {
            this.Calls = calls;
        }

        public IEnumerable<Call> Calls { get; }
    }
}
