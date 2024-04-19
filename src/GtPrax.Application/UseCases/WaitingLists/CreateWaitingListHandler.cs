namespace GtPrax.Application.UseCases.WaitingLists;

using System.Threading;
using System.Threading.Tasks;
using FluentResults;
using GtPrax.Domain.Models;
using GtPrax.Domain.Repositories;
using Mediator;

internal sealed class CreateWaitingListHandler : ICommandHandler<CreateWaitingListCommand, Result>
{
    private readonly TimeProvider _timeProvider;
    private readonly IWaitingListRepo _waitingListRepo;

    public CreateWaitingListHandler(
        TimeProvider timeProvider,
        IWaitingListRepo waitingListRepo)
    {
        _timeProvider = timeProvider;
        _waitingListRepo = waitingListRepo;
    }

    public async ValueTask<Result> Handle(CreateWaitingListCommand command, CancellationToken cancellationToken)
    {
        var waitingListItems = await _waitingListRepo.GetAll(cancellationToken);

        var waitingList = new WaitingList(waitingListItems);
        var result = waitingList.AddItem(command.Name, command.CreatedBy, _timeProvider.GetUtcNow());
        if (result.IsFailed)
        {
            return result.ToResult();
        }

        await _waitingListRepo.Upsert(result.Value, cancellationToken);

        return Result.Ok();
    }
}
