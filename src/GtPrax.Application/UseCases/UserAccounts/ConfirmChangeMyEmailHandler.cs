namespace GtPrax.Application.UseCases.UserAccounts;

using System.Threading;
using System.Threading.Tasks;
using FluentResults;
using GtPrax.Application.Services;
using Mediator;

internal sealed class ConfirmChangeMyEmailHandler : ICommandHandler<ConfirmChangeMyEmailCommand, Result>
{
    private readonly IUserService _userService;

    public ConfirmChangeMyEmailHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async ValueTask<Result> Handle(ConfirmChangeMyEmailCommand command, CancellationToken cancellationToken)
    {
        var result = await _userService.ChangeEmail(command.Id, command.Token, command.NewEmail);
        return result;
    }
}
