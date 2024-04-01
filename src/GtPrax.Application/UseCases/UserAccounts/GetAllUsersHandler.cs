namespace GtPrax.Application.UseCases.UserAccounts;

using System;
using System.Threading;
using System.Threading.Tasks;
using GtPrax.Application.Converter;
using GtPrax.Application.Services;
using Mediator;

internal sealed class GetAllUsersHandler : IQueryHandler<GetAllUsersQuery, UserDto[]>
{
    private readonly TimeProvider _timeProvider;
    private readonly IUserService _userService;

    public GetAllUsersHandler(
        TimeProvider timeProvider,
        IUserService userService)
    {
        _timeProvider = timeProvider;
        _userService = userService;
    }

    public async ValueTask<UserDto[]> Handle(GetAllUsersQuery query, CancellationToken cancellationToken)
    {
        var users = await _userService.GetAll(cancellationToken);
        if (users.Length < 1)
        {
            return [];
        }
        var dateTimeConverter = new GermanDateTimeConverter();
        return users.Select(u => u.MapToDto(dateTimeConverter, _timeProvider)).ToArray();
    }
}
