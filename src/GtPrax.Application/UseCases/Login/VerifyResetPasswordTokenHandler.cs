namespace GtPrax.Application.UseCases.Login;

using System.Threading;
using System.Threading.Tasks;
using FluentResults;
using GtPrax.Application.Identity;
using Mediator;

internal sealed class VerifyResetPasswordTokenHandler : IQueryHandler<VerifyResetPasswordTokenQuery, Result>
{
    private readonly IIdentityService _identityService;

    public VerifyResetPasswordTokenHandler(
        IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async ValueTask<Result> Handle(VerifyResetPasswordTokenQuery query, CancellationToken cancellationToken)
    {
        var result = await _identityService.VerifyResetPasswordToken(query.UserId, query.Token);
        if (!result.Succeeded)
        {
            return Result.Fail(result.Errors.Select(e => e.Description));
        }
        return Result.Ok();
    }
}
