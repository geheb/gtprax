namespace GtPrax.Application.UseCases.Login;

using FluentResults;
using GtPrax.Application.Identity;
using Mediator;

internal sealed class ConfirmEmailHandler : IRequestHandler<ConfirmEmailCommand, Result>
{
    private readonly IIdentityService _identityService;

    public ConfirmEmailHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async ValueTask<Result> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        var result = await _identityService.ConfirmEmail(request.Id, request.Token, request.Password);
        if (!result.Succeeded)
        {
            return Result.Fail(result.Errors.Select(e => e.Description));
        }

        return Result.Ok();
    }
}
