//using System;
//using System.Collections.Generic;
//using System.Text;
//using ValueSet.Abstractions;

//namespace ValueSet.Providers.InMemory
//{
//    public sealed class InMemoryQueryableValueSetExecutor<TRecord> : IQueryableValueSetExecutor<TRecord>
//        where TRecord : class
//    {
//        public Task<int> CountAsync(IQueryable<TRecord> query, CancellationToken cancellationToken)
//        {
//            cancellationToken.ThrowIfCancellationRequested();
//            return Task.FromResult(query.Count());
//        }

//        public Task<IReadOnlyList<TRecord>> ToListAsync(IQueryable<TRecord> query, CancellationToken cancellationToken)
//        {
//            cancellationToken.ThrowIfCancellationRequested();
//            return Task.FromResult<IReadOnlyList<TRecord>>(query.ToList());
//        }
//    }
//}
