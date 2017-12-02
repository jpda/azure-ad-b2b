using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using azure_ad_b2b_entities;
using azure_ad_b2b_services;
using azure_ad_b2b_services.AadTenantRepo;
using azure_ad_b2b_entities.AadTenant;

namespace azure_ad_b2b_service
{
    public class AadService : IAadService
    {
        private readonly IAadRepository _repo;
        public AadService(IAadRepository repo)
        {
            _repo = repo;
        }

        public async Task<AadTenant> GetTenantByIdAsync(string tenantId)
        {
            var result = await _repo.GetTenantByIdAsync(tenantId);
            return result.Success ? new AadTenant(result.Value) : null;
        }

        public async Task<AadUser> GetUserByUpnAsync(string upn)
        {
            var result = await _repo.GetUserByUpnAsync(upn);
            return result.Success ? new AadUser(result.Value) : null;
        }

        public async Task<List<AadTenant>> GetAllTenantsAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<List<AadUser>> GetAllUsersAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<List<AadUser>> GetAllUsersByTenantAsync(string tenantId)
        {
            throw new NotImplementedException();
        }

        public async Task<AadTenant> AddTenantAsync(AadTenant t)
        {
            var result = await _repo.AddTenantAsync(new AadTenantEntity(t.TenantId, t.Issuer, t.AdminConsent));
            return t;
        }

        public async Task<AadTenant> UpdateTenantAsync(AadTenant t)
        {
            await _repo.UpdateTenantAsync(new AadTenantEntity(t.TenantId, t.Issuer, t.AdminConsent));
            return t;
        }

        public async Task<AadUser> AddUserAsync(AadUser u)
        {
            await _repo.AddUserAsync(new AadUserEntity(u.TenantId, u.NameIdentifier, u.Upn));
            return u;
        }

        public async Task<AadUser> UpdateUserAsync(AadUser u)
        {
            await _repo.UpdateUserAsync(new AadUserEntity(u.TenantId, u.NameIdentifier, u.Upn));
            return u;
        }
    }
}
