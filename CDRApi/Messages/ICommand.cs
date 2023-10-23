using MediatR;

namespace CDRApi.Messages
{
    public interface ICommand : IRequest
    {
    }

    public interface ICommand<T> : ICommand, IRequest<T>
    {
    }
}
