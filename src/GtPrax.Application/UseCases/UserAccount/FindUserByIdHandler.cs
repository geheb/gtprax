namespace GtPrax.Application.UseCases.UserAccount;

using System.Threading;
using System.Threading.Tasks;
using GtPrax.Application.Converter;
using GtPrax.Application.Services;
using Mediator;

internal sealed class FindUserByIdHandler : IQueryHandler<FindUserByIdQuery, UserDto?>
{
    private readonly TimeProvider _timeProvider;
    private readonly IUserService _userService;

    public FindUserByIdHandler(
        TimeProvider timeProvider,
        IUserService userService)
    {
        _timeProvider = timeProvider;
        _userService = userService;
    }

    public async ValueTask<UserDto?> Handle(FindUserByIdQuery query, CancellationToken cancellationToken)
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
