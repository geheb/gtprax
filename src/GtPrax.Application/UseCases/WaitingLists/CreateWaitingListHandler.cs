namespace GtPrax.Application.UseCases.WaitingLists;

using System.Threading;
using System.Threading.Tasks;
using FluentResults;
using GtPrax.Domain.Entities;
using Mediator;

internal sealed class CreateWaitingListHandler : ICommandHandler<CreateWaitingListCommand, Result>
{
    private readonly IWaitingListStore _store;

    public CreateWaitingListHandler(IWaitingListStore store)
    {
        _store = store;
    }

    public async ValueTask<Result> Handle(CreateWaitingListCommand command, CancellationToken cancellationToken)
    {
        var waitingListIdentities = await _store.GetIdentities(cancellationToken);

        var result = WaitingList.Create(command.Name, waitingListIdentities);

        if (result.IsFailed)
        {
            return result.ToResult();
        }

        await _store.Create(result.Value, cancellationToken);
        return Result.Ok();
    }
}
