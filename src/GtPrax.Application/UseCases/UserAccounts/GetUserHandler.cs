namespace GtPrax.Application.UseCases.UserAccounts;

using System.Threading;
using System.Threading.Tasks;
using GtPrax.Application.Converter;
using GtPrax.Application.Services;
using Mediator;

internal sealed class GetUserHandler : IQueryHandler<GetUserQuery, UserDto?>
{
    private readonly TimeProvider _timeProvider;
    private readonly IUserService _userService;

    public GetUserHandler(
        TimeProvider timeProvider,
        IUserService userService)
    {
        _timeProvider = timeProvider;
        _userService = userService;
    }

    public async ValueTask<UserDto?> Handle(GetUserQuery query, CancellationToken cancellationToken)
    {
        var user = await _userService.Find(query.Id);
        if (user is null)
        {
            return null;
        }
        var dateConverter = new GermanDateTimeConverter();
        return user.MapToDto(dateConverter, _timeProvider);
    }
}
