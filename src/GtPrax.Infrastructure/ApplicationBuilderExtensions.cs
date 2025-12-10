namespace GtPrax.Infrastructure;

using GtPrax.Application.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

public static class ApplicationBuilderExtensions
{
    public static void UseNodeGenerator(this IApplicationBuilder builder)
    {
        var nodeGenerator = builder.ApplicationServices.GetRequiredService<INodeGenerator>();
        nodeGenerator.BuildNodes();
    }
}
