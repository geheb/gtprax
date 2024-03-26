namespace GtPrax.Application.UseCases.UsersManagement;

using System.Threading;
using System.Threading.Tasks;
using FluentResults;
using GtPrax.Application.Email;
using GtPrax.Application.Identity;
using Mediator;
using Microsoft.AspNetCore.Identity;

internal sealed class CreateUserHandler : IRequestHandler<CreateUserCommand, Result>
{
    private readonly IdentityErrorDescriber _identityErrorDescriber;
    private readonly IEmailValidatorService _emailValidatorService;
    private readonly IEmailQueueService _emailQueueService;
    private readonly IIdentityService _identityService;

    public CreateUserHandler(
        IdentityErrorDescriber identityErrorDescriber,
        IEmailValidatorService emailValidatorService,
        IEmailQueueService emailQueueService,
        IIdentityService identityService)
    {
        _identityErrorDescriber = identityErrorDescriber;
        _emailValidatorService = emailValidatorService;
        _emailQueueService = emailQueueService;
        _identityService = identityService;
    }

    public async ValueTask<Result> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var isValid = await _emailValidatorService.Validate(request.Email, cancellationToken);
        if (!isValid)
        {
            return Result.Fail(_identityErrorDescriber.InvalidEmail(request.Email).Description);
        }

        var result = await _identityService.Create(request.Email, request.Name, request.Roles);
        if (!result.Succeeded)
        {
            return Result.Fail(result.Errors.Select(e => e.Description));
        }

        var user = await _identityService.FindUserByEmail(request.Email);
        if (user is null)
        {
            return Result.Fail(_identityErrorDescriber.DefaultError().Description);
        }

        var token = await _identityService.GenerateConfirmEmailToken(user.Id);
        var callbackUrl = new UriBuilder(request.CallbackUrl)
        {
            Query = $"id={user.Id}&token={token}"
        }.ToString();

        await _emailQueueService.EnqueueConfirmEmail(request.Email, request.Name, callbackUrl, cancellationToken);

        return Result.Ok();
    }
}
