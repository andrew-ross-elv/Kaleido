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
        services.TryAddSingleton<IRecordMetadataCatalog, RecordMetadataCatalog>();
        services.TryAddSingleton<IRecordDescriptorFactory, RecordDescriptorFactory>();
        services.TryAddSingleton<IRecordQueryValidator, RecordQueryValidator>();
        services.TryAddSingleton<IRecordQueryCompiler, RecordQueryCompiler>();
        services.TryAddSingleton<IRecordRegistry, RecordRegistry>();
        services.TryAddScoped<IRecordDispatcher, RecordDispatcher>();
        services.TryAddScoped<IKaleidoCatalog, KaleidoCatalog>();
        return services;
    }

    /// <summary>Adds services required for queryable value-set execution.</summary>
    /// <param name="services">Service collection to register with.</param>
    /// <returns>The same service collection for chaining.</returns>
    public static IServiceCollection AddRecordQueryable(this IServiceCollection services)
    {
        services.AddKaleido();
        services.TryAddSingleton(typeof(IQueryableCompiledQueryApplier<>), typeof(QueryableCompiledQueryApplier<>));
        services.TryAddSingleton(typeof(IQueryableRecordExecutor<>), typeof(QueryableRecordExecutor<>));
        return services;
    }

    /// <summary>Scans an assembly for value-set sources and named queries.</summary>
    /// <param name="services">Service collection to register with.</param>
    /// <param name="assembly">Assembly that contains value-set sources and named queries.</param>
    /// <param name="lifetime">Lifetime used for discovered sources and named queries.</param>
    /// <returns>The same service collection for chaining.</returns>
    public static IServiceCollection AddQueryableRecordsFromAssembly(this IServiceCollection services, Assembly assembly, ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        services.AddRecordQueryable();
        foreach (var type in assembly.DefinedTypes.Where(x => x.IsClass && !x.IsAbstract).Select(x => x.AsType()))
        {
            foreach (var sourceInterface in type.GetInterfaces().Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IQueryableRecordSource<>)))
            {
                var recordType = sourceInterface.GenericTypeArguments[0];
                var metadata = RecordMetadataBuilder.Build(recordType);
                services.AddSingleton(new RecordRegistration(metadata.Name, recordType, metadata));
                services.Add(new ServiceDescriptor(sourceInterface, type, lifetime));
                services.TryAdd(new ServiceDescriptor(typeof(IRecordQueryEngine<>).MakeGenericType(recordType), typeof(QueryableRecordSetQueryEngine<>).MakeGenericType(recordType), lifetime));
            }
            foreach (var namedQueryInterface in type.GetInterfaces().Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IQueryableRecordNamedQuery<>)))
            {
                services.Add(new ServiceDescriptor(namedQueryInterface, type, lifetime));
            }
        }
        return services;
    }
}
