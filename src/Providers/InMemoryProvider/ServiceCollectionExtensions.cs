//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.DependencyInjection.Extensions;
//using ValueSet.Abstractions;
//using ValueSet.Queryable;

//namespace ValueSet.InMemory;

//public static class ValueSetInMemoryServiceCollectionExtensions
//{
//    public static IServiceCollection AddInMemoryValueSet<TRecord>(
//        this IServiceCollection services)
//        where TRecord : class
//    {
//        services.AddValueSetQueryable();

//        services.AddSingleton<IQueryableValueSetSource<TRecord>>(
//            new InMemoryValueSetSource<TRecord>());

//        return services;
//    }
//}