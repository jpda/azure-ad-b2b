using azure_ad_b2b_services.AadTenantRepo;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace azure_ad_b2b_services
{
    interface IAadService
    {
        Task<AadTenant> GetTenantByIdAsync(string tenantId);
        Task<AadUser> GetUserByUpnAsync(string upn);
        Task<List<AadTenant>> GetAllTenantsAsync();
        Task<List<AadUser>> GetAllUsersAsync();
        Task<List<AadUser>> GetAllUsersByTenantAsync(string tenantId);
        Task<AadTenant> AddTenantAsync(AadTenant t);
        Task<AadTenant> UpdateTenantAsync(AadTenant t);
        Task<AadUser> AddUserAsync(AadUser t);
        Task<AadUser> UpdateUserAsync(AadUser t);
    }
}