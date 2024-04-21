namespace GtPrax.Application.UseCases.UserAccount;

using Mediator;

public sealed record FindUserByIdQuery(string Id) : IQuery<UserDto?>;
