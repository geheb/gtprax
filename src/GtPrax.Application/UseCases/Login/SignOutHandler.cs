namespace GtPrax.Application.UseCases.Login;

using System.Threading;
using System.Threading.Tasks;
using FluentResults;
using GtPrax.Application.Identity;
using Mediator;

internal sealed class SignOutHandler : IRequestHandler<SignOutCommand, Result>
{
    private readonly IIdentityService _identityService;

    public SignOutHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async ValueTask<Result> Handle(SignOutCommand request, CancellationToken cancellationToken)
    {
        await _identityService.SignOutCurrentUser();
        return Result.Ok();
    }
}
