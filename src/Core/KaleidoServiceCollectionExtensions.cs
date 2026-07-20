using Kaleido.Metadata;
using Kaleido.Queryable;
using Kaleido.Registry;
using Kaleido.Validation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Reflection;

namespace Kaleido;

public static class KaleidoServiceCollectionExtensions
{
    /// <summary>Adds core framework services.</summary>
    /// <param name="services">Service collection to add registrations to.</param>
    /// <returns>The same service collection for chaining.</returns>
    public static IServiceCollection AddKaleido(this IServiceCollection services)
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

    /// <summary>Adds services required for queryable value-set execution.</summary>
    /// <param name="services">Service collection to register with.</param>
    /// <returns>The same service collection for chaining.</returns>
    public static IServiceCollection AddValueSetQueryable(this IServiceCollection services)
    {
        services.AddKaleido();
        services.TryAddSingleton(typeof(IQueryableCompiledQueryApplier<>), typeof(QueryableCompiledQueryApplier<>));
        services.TryAddSingleton(typeof(IQueryableValueSetExecutor<>), typeof(QueryableValueSetExecutor<>));
        return services;
    }

    /// <summary>Scans an assembly for value-set sources and named queries.</summary>
    /// <param name="services">Service collection to register with.</param>
    /// <param name="assembly">Assembly that contains value-set sources and named queries.</param>
    /// <param name="lifetime">Lifetime used for discovered sources and named queries.</param>
    /// <returns>The same service collection for chaining.</returns>
    public static IServiceCollection AddQueryableValueSetsFromAssembly(this IServiceCollection services, Assembly assembly, ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        services.AddValueSetQueryable();
        foreach (var type in assembly.DefinedTypes.Where(x => x.IsClass && !x.IsAbstract).Select(x => x.AsType()))
        {
            foreach (var sourceInterface in type.GetInterfaces().Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IQueryableValueSetSource<>)))
            {
                var recordType = sourceInterface.GenericTypeArguments[0];
                var metadata = ValueSetMetadataBuilder.Build(recordType);
                services.AddSingleton(new ValueSetRegistration(metadata.Name, recordType, metadata));
                services.Add(new ServiceDescriptor(sourceInterface, type, lifetime));
                services.TryAdd(new ServiceDescriptor(typeof(IValueSetQueryEngine<>).MakeGenericType(recordType), typeof(QueryableValueSetQueryEngine<>).MakeGenericType(recordType), lifetime));
            }
            foreach (var namedQueryInterface in type.GetInterfaces().Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IQueryableValueSetNamedQuery<>)))
            {
                services.Add(new ServiceDescriptor(namedQueryInterface, type, lifetime));
            }
        }
        return services;
    }
}
