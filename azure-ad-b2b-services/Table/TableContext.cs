using System;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using azure_ad_b2b_shared;
using System.Collections.Generic;
using Microsoft.Extensions.Options;

namespace azure_ad_b2b_services
{
    public class AuthTableContext : TableContext, IAuthTableContext
    {
        public AuthTableContext(IOptions<StorageOptions> storageOptions) : base(storageOptions, Constants.AuthTableName) { }
    }

    public class UserTableContext : TableContext, IUserTableContext
    {
        public UserTableContext(IOptions<StorageOptions> storageOptions) : base(storageOptions, Constants.UserTableName) { }
    }

    public class TenantTableContext : TableContext, ITenantTableContext
    {
        public TenantTableContext(IOptions<StorageOptions> storageOptions) : base(storageOptions, Constants.TenantTableName) { }
    }

    public interface ITenantTableContext : ITableContext { }
    public interface IUserTableContext : ITableContext { }
    public interface IAuthTableContext : ITableContext { }

    public class TableContext : ITableContext
    {
        private readonly CloudTableClient _ctc;
        private readonly CloudTable _table;

        public TableContext(IOptions<StorageOptions> storageOptions, string tableName)
        {
            var a = CloudStorageAccount.Parse(storageOptions.Value.ConnectionString);
            _ctc = a.CreateCloudTableClient();
            _table = GetTableByName(tableName).Result; //sync, blocking
        }

        private async Task<CloudTable> GetTableByName(string name)
        {
            var table = _ctc.GetTableReference(name);
            await table.CreateIfNotExistsAsync();
            return table;
        }

        public async Task<ServiceResult<T>> RetrieveEntityAsync<T>(string pkey, string rkey) where T : TableEntity
        {
            var o = TableOperation.Retrieve<T>(pkey, rkey);
            var result = await _table.ExecuteAsync(o);
            if (result.HttpStatusCode != 200)
            {
                return new ServiceResult<T>().ToErrorResult($"{result.HttpStatusCode}");
            }
            return new ServiceResult<T>(result.Result as T);
        }

        public async Task<ServiceResult<T>> RetrieveEntityAsync<T>(T entity) where T : TableEntity
        {
            return await RetrieveEntityAsync<T>(entity.PartitionKey, entity.RowKey);
        }

        public async Task<ServiceResult<IList<T>>> RetrievePartitionAsync<T>(string partition) where T : TableEntity, new()
        {
            TableContinuationToken token = null;
            var e = new List<T>();
            do
            {
                var q = await _table.ExecuteQuerySegmentedAsync<T>(new TableQuery<T>() { FilterString = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partition) }, token);
                e.AddRange(q.Results);
                token = q.ContinuationToken;

            } while (token != null);
            return new ServiceResult<IList<T>>(e);
        }

        /// <summary>
        /// Merges entity based on Pkey and Rkey
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<ServiceResult<T>> SaveOrMergeEntityAsync<T>(T value) where T : TableEntity
        {
            if (string.IsNullOrEmpty(value.PartitionKey)) { value.PartitionKey = value.GetType().Name; }
            var a = TableOperation.InsertOrMerge(value);
            var result = await _table.ExecuteAsync(a);
            if (result.HttpStatusCode > 204)
            {
                return new ServiceResult<T>().ToErrorResult($"{result.HttpStatusCode}");
            }
            return new ServiceResult<T>(result.Result as T);
        }
    }
}