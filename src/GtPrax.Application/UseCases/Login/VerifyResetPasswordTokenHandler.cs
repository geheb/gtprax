namespace GtPrax.Application.UseCases.Login;

using System.Threading;
using System.Threading.Tasks;
using FluentResults;
using GtPrax.Application.Services;
using Mediator;

internal sealed class VerifyResetPasswordTokenHandler : IQueryHandler<VerifyResetPasswordTokenQuery, Result>
{
    private readonly IUserService _userService;

    public VerifyResetPasswordTokenHandler(
        IUserService userService)
    {
        _userService = userService;
    }

    public async ValueTask<Result> Handle(VerifyResetPasswordTokenQuery query, CancellationToken cancellationToken)
    {
        var result = await _userService.VerifyResetPasswordToken(query.Id, query.Token);
        return result;
    }
}
