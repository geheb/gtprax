namespace GtPrax.Application.UseCases.MyAccount;

using Mediator;

public sealed record GetMyUserQuery(string UserId) : IQuery<MyUserDto?>;
