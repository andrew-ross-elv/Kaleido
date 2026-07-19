using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Kaleido.Abstractions;

namespace Kaleido.Core;

public static class ValueSetCoreServiceCollectionExtensions
{
    /// <summary>Adds core framework services.</summary>
    /// <param name="services">Service collection to add registrations to.</param>
    /// <returns>The same service collection for chaining.</returns>
    public static IServiceCollection AddValueSetCore(this IServiceCollection services)
    {
        services.TryAddSingleton<IValueSetMetadataCatalog, ValueSetMetadataCatalog>();
        services.TryAddSingleton<IValueSetDescriptorFactory, ValueSetDescriptorFactory>();
        services.TryAddSingleton<IValueSetQueryValidator, ValueSetQueryValidator>();
        services.TryAddSingleton<IValueSetQueryCompiler, ValueSetQueryCompiler>();
        services.TryAddSingleton<IValueSetRegistry, ValueSetRegistry>();
        services.TryAddScoped<IValueSetDispatcher, ValueSetDispatcher>();
        services.TryAddScoped<IValueSetCatalog, ValueSetCatalog>();
        return services;
    }
}
