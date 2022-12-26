using MediatR;

namespace Kozachok.Shared.Abstractions.Commands
{
    public abstract class RequestCommand<TResult> : Command, IRequest<TResult>
    {
    }
}
