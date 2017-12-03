using System.Threading.Tasks;
using azure_ad_b2b_shared;
using azure_ad_b2b_entities.AppTenant;
using System.Collections.Generic;

namespace azure_ad_b2b_services.AppTenantRepo
{
    public interface IAppRepository
    {
        Task<ServiceResult<AppUserEntity>> GetUserByUpnAsync(string tenantId, string upn);
        Task<ServiceResult<AppTenantEntity>> GetTenantByIdAsync(string tenantId);
        Task<ServiceResult<AppTenantEntity>> AddTenantAsync(AppTenantEntity t);
        Task<ServiceResult<AppTenantEntity>> UpdateTenantAsync(AppTenantEntity t);
        Task<ServiceResult<AppUserEntity>> AddUserAsync(AppUserEntity t);
        Task<ServiceResult<AppUserEntity>> UpdateUserAsync(AppUserEntity t);
        Task<ServiceResult<IList<AppTenantEntity>>> GetAllTenantsAsync();
        Task<ServiceResult<IList<AppUserEntity>>> GetUsersByTenantIdAsync(string tenantId);
    }
}