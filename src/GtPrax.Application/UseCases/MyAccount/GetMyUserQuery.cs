namespace GtPrax.Application.UseCases.MyAccount;

using GtPrax.Application.UseCases.UsersManagement;
using Mediator;

public sealed record GetMyUserQuery(string UserId) : IQuery<UserDto?>;
