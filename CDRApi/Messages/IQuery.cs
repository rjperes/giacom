using MediatR;

namespace CDRApi.Messages
{
    public interface IQuery : IRequest
    {
    }

    public interface IQuery<T> : IRequest<T>, IQuery
    {
    }
}
