namespace GtPrax.Application.UseCases.Login;

using System.Threading;
using System.Threading.Tasks;
using FluentResults;
using GtPrax.Application.Services;
using Mediator;

internal sealed class VerifyConfirmEmailTokenHandler : IQueryHandler<VerifyConfirmEmailTokenQuery, Result>
{
    private readonly IUserService _userService;

    public VerifyConfirmEmailTokenHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async ValueTask<Result> Handle(VerifyConfirmEmailTokenQuery query, CancellationToken cancellationToken)
    {
        var result = await _userService.VerifyConfirmEmailToken(query.Id, query.Token);
        return result;
    }
}
