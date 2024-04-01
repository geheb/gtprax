namespace GtPrax.Application.UseCases.UserAccounts;

using System.Threading;
using System.Threading.Tasks;
using FluentResults;
using GtPrax.Application.Services;
using GtPrax.Domain.ValueObjects;
using Mediator;
using Microsoft.AspNetCore.Identity;

internal sealed class CreateUserHandler : ICommandHandler<CreateUserCommand, Result>
{
    private readonly IdentityErrorDescriber _errorDescriber;
    private readonly IEmailValidatorService _emailValidatorService;
    private readonly IEmailQueueService _emailQueueService;
    private readonly IUserService _userService;

    public CreateUserHandler(
        IdentityErrorDescriber errorDescriber,
        IEmailValidatorService emailValidatorService,
        IEmailQueueService emailQueueService,
        IUserService userService)
    {
        _errorDescriber = errorDescriber;
        _emailValidatorService = emailValidatorService;
        _emailQueueService = emailQueueService;
        _userService = userService;
    }

    public async ValueTask<Result> Handle(CreateUserCommand command, CancellationToken cancellationToken)
    {
        var isValid = await _emailValidatorService.Validate(command.Email, cancellationToken);
        if (!isValid)
        {
            return Result.Fail(_errorDescriber.InvalidEmail(command.Email).Description);
        }

        var roles = UserRoleType.From(command.Roles.Cast<int>());
        var result = await _userService.Create(command.Email, command.Name, roles);
        if (result.IsFailed)
        {
            return result;
        }

        var user = await _userService.FindByEmail(command.Email);
        if (user is null)
        {
            return Result.Fail(_errorDescriber.DefaultError().Description);
        }

        var token = await _userService.GenerateConfirmEmailToken(user.Id);
        var callbackUrl = new UriBuilder(command.CallbackUrl)
        {
            Query = $"id={user.Id}&token={token}"
        }.ToString();

        await _emailQueueService.EnqueueConfirmEmail(command.Email, command.Name, callbackUrl, cancellationToken);

        return Result.Ok();
    }
}
