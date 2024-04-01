namespace GtPrax.Application.UseCases.UserAccounts;

using System.Threading;
using System.Threading.Tasks;
using FluentResults;
using GtPrax.Application.Services;
using Mediator;

internal sealed class DeactivateUserHandler : ICommandHandler<DeactivateUserCommand, Result>
{
    private readonly IUserService _userService;

    public DeactivateUserHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async ValueTask<Result> Handle(DeactivateUserCommand command, CancellationToken cancellationToken)
    {
        var result = await _userService.Deactivate(command.Id);
        return result;
    }
}
