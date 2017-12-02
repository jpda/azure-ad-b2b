using System;
using System.Threading.Tasks;
using azure_ad_b2b_entities;
using azure_ad_b2b_shared;
using azure_ad_b2b_entities.AppTenant;
using System.Collections.Generic;

namespace azure_ad_b2b_services.AppTenantRepo
{
    public class AppRepository : IAppRepository
    {
        private readonly ITenantTableContext _tenantCtx;
        private readonly IUserTableContext _userCtx;

        public AppRepository(ITenantTableContext tenantCtx, IUserTableContext userCtx)
        {
            _tenantCtx = tenantCtx;
            _userCtx = userCtx;
        }

        public async Task<ServiceResult<AppTenantEntity>> SaveTenantAsync(AppTenantEntity t)
        {
            var a = await _tenantCtx.SaveOrMergeEntityAsync(t);
            return a;
        }

        public async Task<ServiceResult<IList<AppTenantEntity>>> GetAllTenantsAsync()
        {
            var a = await _tenantCtx.RetrievePartitionAsync<AppTenantEntity>("AppTenantEntity");
            return a;
        }

        public async Task<ServiceResult<AppUserEntity>> GetUserByUpnAsync(string tenantId, string upn)
        {
            return await _userCtx.RetrieveEntityAsync<AppUserEntity>(tenantId, upn);
        }

        public async Task<ServiceResult<AppTenantEntity>> GetTenantByIdAsync(string tenantId)
        {
            return await _tenantCtx.RetrieveEntityAsync<AppTenantEntity>(new AppTenantEntity(tenantId));
        }

        public async Task<ServiceResult<AppTenantEntity>> AddTenantAsync(AppTenantEntity t)
        {
            return await _tenantCtx.SaveOrMergeEntityAsync(t);
        }

        public async Task<ServiceResult<AppTenantEntity>> UpdateTenantAsync(AppTenantEntity t)
        {
            return await _tenantCtx.SaveOrMergeEntityAsync(t);
        }

        public async Task<ServiceResult<AppUserEntity>> AddUserAsync(AppUserEntity t)
        {
            return await _userCtx.SaveOrMergeEntityAsync(t);
        }

        public async Task<ServiceResult<AppUserEntity>> UpdateUserAsync(AppUserEntity t)
        {
            return await _userCtx.SaveOrMergeEntityAsync(t);
        }
    }
}
