using azure_ad_b2b_entities.AadTenant;
using azure_ad_b2b_shared;
using Microsoft.WindowsAzure.Storage.Table;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace azure_ad_b2b_entities
{
    public interface ITableContext
    {
        Task<ServiceResult<T>> RetrieveEntityAsync<T>(string pkey, string rowkey) where T : TableEntity;
        Task<ServiceResult<T>> RetrieveEntityAsync<T>(T entity) where T : TableEntity;
        Task<ServiceResult<T>> SaveOrMergeEntityAsync<T>(T value) where T : TableEntity;
        Task<ServiceResult<IList<T>>> RetrievePartitionAsync<T>(string partition) where T : TableEntity, new();
    }
}