using GtPrax.Application.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace GtPrax.Application;

public static class ServiceExtensions
{
	public static IServiceCollection AddApplication(this IServiceCollection services)
	{
		return services;
	}
}
