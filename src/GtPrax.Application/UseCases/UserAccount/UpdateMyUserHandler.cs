namespace GtPrax.Application.UseCases.UserAccount;

using System.Threading;
using System.Threading.Tasks;
using FluentResults;
using GtPrax.Application.Services;
using Mediator;

internal sealed class UpdateMyUserHandler : ICommandHandler<UpdateMyUserCommand, Result>
{
    private readonly IUserService _userService;

    public UpdateMyUserHandler(
        IUserService userService)
    {
        _userService = userService;
    }

    public async ValueTask<Result> Handle(UpdateMyUserCommand command, CancellationToken cancellationToken)
    {
        var result = await _userService.SetName(command.Id, command.Name, cancellationToken);
        return result;
    }
}
