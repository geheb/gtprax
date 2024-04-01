namespace GtPrax.Application.UseCases.Login;

using FluentResults;
using GtPrax.Application.Services;
using Mediator;

internal sealed class ConfirmEmailHandler : ICommandHandler<ConfirmEmailCommand, Result>
{
    private readonly IUserService _userService;

    public ConfirmEmailHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async ValueTask<Result> Handle(ConfirmEmailCommand command, CancellationToken cancellationToken)
    {
        var result = await _userService.ConfirmEmail(command.Id, command.Token, command.Password);
        return result;
    }
}
