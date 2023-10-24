using CDRApi.Messages;
using MediatR;

namespace CDRApi.Handlers
{
    public class FileUploadedHandler : INotificationHandler<FileUploadedNotification>
    {
        private readonly ILogger<FileUploadedHandler> _logger;

        public FileUploadedHandler(ILogger<FileUploadedHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(FileUploadedNotification notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("A file was just uploaded");

            return Task.CompletedTask;
        }
    }
}
