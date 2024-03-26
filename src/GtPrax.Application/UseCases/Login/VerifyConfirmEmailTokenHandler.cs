namespace GtPrax.Application.UseCases.Login;

using System.Threading;
using System.Threading.Tasks;
using FluentResults;
using GtPrax.Application.Identity;
using Mediator;

internal sealed class VerifyConfirmEmailTokenHandler : IQueryHandler<VerifyConfirmEmailTokenQuery, Result>
{
    private readonly IIdentityService _identityService;

    public VerifyConfirmEmailTokenHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async ValueTask<Result> Handle(VerifyConfirmEmailTokenQuery query, CancellationToken cancellationToken)
    {
        var result = await _identityService.VerifyConfirmEmailToken(query.Id, query.Token);
        if (!result.Succeeded)
        {
            return Result.Fail(result.Errors.Select(e => e.Description));
        }
        return Result.Ok();
    }
}
