using System;
using System.Threading.Tasks;
using azure_ad_b2b_entities;
using azure_ad_b2b_shared;
using azure_ad_b2b_entities.AadTenant;

namespace azure_ad_b2b_services.AadTenantRepo
{
    public class AadRepository : IAadRepository
    {
        private readonly IAuthTableContext _ctx;
        public AadRepository(IAuthTableContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<ServiceResult<AadTenantEntity>> SaveTenantAsync(AadTenantEntity t)
        {
            var a = await _ctx.SaveOrMergeEntityAsync(t);
            return a;
        }

        public async Task<ServiceResult<AadUserEntity>> GetUserByUpnAsync(string upn)
        {
            return await _ctx.RetrieveEntityAsync<AadUserEntity>("UserEntity", upn);
        }

        public async Task<ServiceResult<AadTenantEntity>> GetTenantByIdAsync(string tenantId)
        {
            return await _ctx.RetrieveEntityAsync<AadTenantEntity>(new AadTenantEntity(tenantId));
        }

        public async Task<ServiceResult<AadTenantEntity>> AddTenantAsync(AadTenantEntity t)
        {
            return await _ctx.SaveOrMergeEntityAsync(t);
        }

        public async Task<ServiceResult<AadTenantEntity>> UpdateTenantAsync(AadTenantEntity t)
        {
            return await _ctx.SaveOrMergeEntityAsync(t);
        }

        public async Task<ServiceResult<AadUserEntity>> AddUserAsync(AadUserEntity t)
        {
            return await _ctx.SaveOrMergeEntityAsync(t);
        }

        public async Task<ServiceResult<AadUserEntity>> UpdateUserAsync(AadUserEntity t)
        {
            return await _ctx.SaveOrMergeEntityAsync(t);
        }
    }
}
