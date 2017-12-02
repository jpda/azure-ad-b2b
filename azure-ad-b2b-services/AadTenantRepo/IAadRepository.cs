using System;
using System.Threading.Tasks;
using azure_ad_b2b_entities;
using azure_ad_b2b_shared;
using azure_ad_b2b_entities.AadTenant;

namespace azure_ad_b2b_services.AadTenantRepo
{
    public interface IAadRepository
    {
        Task<ServiceResult<AadUserEntity>> GetUserByUpnAsync(string upn);
        Task<ServiceResult<AadTenantEntity>> GetTenantByIdAsync(string tenantId);
        Task<ServiceResult<AadTenantEntity>> AddTenantAsync(AadTenantEntity t);
        Task<ServiceResult<AadTenantEntity>> UpdateTenantAsync(AadTenantEntity t);
        Task<ServiceResult<AadTenantEntity>> AddUserAsync(AadUserEntity t);
        Task<ServiceResult<AadUserEntity>> UpdateUserAsync(AadUserEntity t);
    }
}