namespace GtPrax.Application.UseCases.Login;

using FluentResults;
using GtPrax.Application.Identity;
using Mediator;

internal sealed class SignInHandler : IRequestHandler<SignInCommand, Result>
{
    private readonly IIdentityService _identityService;

    public SignInHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async ValueTask<Result> Handle(SignInCommand request, CancellationToken cancellationToken)
    {
        var result = await _identityService.SignIn(request.Email, request.Password, cancellationToken);
        if (!result.Succeeded)
        {
            return Result.Fail(result.Errors.Select(e => e.Description));
        }
        return Result.Ok();
    }
}
