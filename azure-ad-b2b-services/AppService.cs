using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using azure_ad_b2b_services.AppTenantRepo;
using azure_ad_b2b_entities.AppTenant;
using System.Linq;

namespace azure_ad_b2b_services
{
    public class AppService : IAppService
    {
        private readonly IAppRepository _repo;
        private readonly IGraphService _graph;
        public AppService(IAppRepository repo, IGraphService graph)
        {
            _repo = repo;
            _graph = graph;
        }

        public async Task<AppTenant> GetTenantByIdAsync(string tenantId)
        {
            var result = await _repo.GetTenantByIdAsync(tenantId);
            return result.Success ? new AppTenant(result.Value) : null;
        }

        public async Task<AppUser> GetUserByUpnAsync(string tenantId, string upn)
        {
            var result = await _repo.GetUserByUpnAsync(tenantId, upn);
            return result.Success ? new AppUser(result.Value) : null;
        }

        public async Task<List<AppTenant>> GetAllTenantsAsync()
        {
            var result = await _repo.GetAllTenantsAsync();
            return result.Success ? result.Value.Select(x => new AppTenant(x)).ToList() : new List<AppTenant>();
        }

        public async Task<List<AppUser>> GetAllUsersByTenantAsync(string tenantId)
        {
            var result = await _repo.GetUsersByTenantIdAsync(tenantId);
            return result.Success ? result.Value.Select(x => new AppUser(x)).ToList() : new List<AppUser>();
        }

        public async Task<AppTenant> AddTenantAsync(AppTenant t)
        {
            var result = await _repo.AddTenantAsync(new AppTenantEntity(t.TenantId, t.Name, t.AdminEmail, t.InvitedBy, string.Empty));
            var user = new AppUser()
            {
                AddedBy = t.InvitedBy,
                DateAdded = t.DateAdded,
                TenantId = t.TenantId,
                Email = t.AdminEmail,
            };
            var u = await AddUserAsync(user, true);
            result.Value.InviteRedeemUrl = u.InviteRedeemUrl;
            await UpdateTenantEntityAsync(result.Value);
            return t;
        }

        public async Task<AppTenant> UpdateTenantAsync(AppTenant t)
        {
            var e = new AppTenantEntity(t.TenantId, t.Name, t.AdminEmail, t.InvitedBy, t.InviteRedeemUrl);
            var thing = await UpdateTenantEntityAsync(e);
            return t;
        }

        private async Task<AppTenant> UpdateTenantEntityAsync(AppTenantEntity t)
        {
            var thing = await _repo.UpdateTenantAsync(t);
            return thing.Success ? new AppTenant(thing.Value) : new AppTenant(t);
        }

        public async Task<AppUser> AddUserAsync(AppUser u, bool invite = false)
        {
            var user = await _repo.AddUserAsync(new AppUserEntity(u.TenantId, u.Email) { DisplayName = u.DisplayName, AddedBy = u.AddedBy, DateAdded = u.DateAdded, NameIdentifier = u.NameIdentifier, InviteRedeemUrl = u.InviteRedeemUrl, Upn = u.Upn });
            if (invite)
            {
                var inviteResult = await _graph.InviteUser(u.Email, invite, u.DisplayName);
                user.Value.InviteRedeemUrl = inviteResult;
                await _repo.UpdateUserAsync(user.Value);
                u.InviteRedeemUrl = user.Value.InviteRedeemUrl;
            }
            return user.Success ? new AppUser(user.Value) : u;
        }

        public async Task<AppUser> UpdateUserAsync(AppUser u)
        {
            return await AddUserAsync(u, false);
        }
    }
}