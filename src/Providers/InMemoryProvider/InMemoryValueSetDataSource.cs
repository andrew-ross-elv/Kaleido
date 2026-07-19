//using System;
//using System.Collections.Generic;
//using System.Text;
//using ValueSet.Abstractions;

//namespace ValueSet.Providers.InMemory
//{
//    public sealed class InMemoryValueSetDataSource<TRecord>
//        : IValueSetDataSource<TRecord>
//        where TRecord : class
//    {
//        private readonly IReadOnlyList<TRecord> _records;

//        public InMemoryValueSetDataSource(IEnumerable<TRecord> records)
//        {
//            _records = records.ToList();
//        }

//        public ValueTask<IEnumerable<TRecord>> GetRecordsAsync(
//            CancellationToken cancellationToken = default)
//        {
//            cancellationToken.ThrowIfCancellationRequested();

//            return ValueTask.FromResult<IEnumerable<TRecord>>(_records);
//        }
//    }
//}
