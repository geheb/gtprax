namespace GtPrax.Application.UseCases.UserAccounts;

using Mediator;

public sealed record GetUserQuery(string Id) : IQuery<UserDto?>;
