using azure_ad_b2b_services.AppTenantRepo;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace azure_ad_b2b_services
{
    public interface IAppService
    {
        Task<AppTenant> GetTenantByIdAsync(string tenantId);
        Task<AppUser> GetUserByUpnAsync(string tenantId, string upn);
        Task<List<AppTenant>> GetAllTenantsAsync();
        Task<List<AppUser>> GetAllUsersByTenantAsync(string tenantId);
        Task<AppTenant> AddTenantAsync(AppTenant t);
        Task<AppTenant> UpdateTenantAsync(AppTenant t);
        Task<AppUser> AddUserAsync(AppUser t, bool invite);
        Task<AppUser> UpdateUserAsync(AppUser t);
    }
}