namespace GtPrax.Application.UseCases.WaitingLists;

using System.Threading;
using System.Threading.Tasks;
using FluentResults;
using GtPrax.Domain.Entities;
using Mediator;

internal sealed class CreateWaitingListHandler : ICommandHandler<CreateWaitingListCommand, Result>
{
    private readonly TimeProvider _timeProvider;
    private readonly IWaitingListStore _store;

    public CreateWaitingListHandler(
        TimeProvider timeProvider,
        IWaitingListStore store)
    {
        _timeProvider = timeProvider;
        _store = store;
    }

    public async ValueTask<Result> Handle(CreateWaitingListCommand command, CancellationToken cancellationToken)
    {
        var waitingListIdentities = await _store.GetIdentities(cancellationToken);

        var waitingListResult = new WaitingListBuilder()
            .SetName(command.Name)
            .SetCreated(command.CreatedBy, _timeProvider.GetUtcNow())
            .Build(waitingListIdentities);

        if (waitingListResult.IsFailed)
        {
            return waitingListResult.ToResult();
        }

        await _store.Upsert(waitingListResult.Value, cancellationToken);
        return Result.Ok();
    }
}
