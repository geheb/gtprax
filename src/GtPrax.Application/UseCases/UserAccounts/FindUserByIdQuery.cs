namespace GtPrax.Application.UseCases.UserAccounts;

using Mediator;

public sealed record FindUserByIdQuery(string Id) : IQuery<UserDto?>;
