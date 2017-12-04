using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using azure_ad_b2b_services.AppTenantRepo;
using azure_ad_b2b_entities.AppTenant;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Xml.Linq;
using azure_ad_b2b_shared;
using System.Security;
using Microsoft.ApplicationInsights;

namespace azure_ad_b2b_services
{
    public class AppService : InstrumentedService, IAppService
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
            var u = await AddUserAsync(user, true, true);
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

        // todo: refactor this
        // todo: fix domain validation
        public async Task<AppUser> AddUserAsync(AppUser u, bool emailInvite = false, bool isCustomerAdmin = false, bool validateDomain = false)
        {
            if (validateDomain)
            {
                var emailPieces = u.Email.Split('@');
                var valid = ValidateTenantAgainstDomain(emailPieces[1], u.TenantId);
                if (!valid) throw new SecurityException("User admin tenant does not match added user tenant");
            }
            var user = await _repo.AddUserAsync(new AppUserEntity(u.TenantId, u.Email)
            {
                DisplayName = u.DisplayName,
                AddedBy = u.AddedBy,
                DateAdded = u.DateAdded,
                NameIdentifier = u.NameIdentifier,
                InviteRedeemUrl = u.InviteRedeemUrl,
                InvitedUserId = u.InvitedUserId,
                Upn = u.Upn
            });

            var inviteResult = await _graph.InviteUser(u.Email, emailInvite, u.DisplayName);
            user.Value.InviteRedeemUrl = inviteResult.InvitedUserInviteRedeemUrl;
            user.Value.InvitedUserId = inviteResult.InvitedUserId;
            await _repo.UpdateUserAsync(user.Value);
            u.InviteRedeemUrl = user.Value.InviteRedeemUrl;
            await _graph.AddUserToRole(inviteResult.InvitedUserId, isCustomerAdmin);

            return user.Success ? new AppUser(user.Value) : u;
        }

        public async Task<AppUser> UpdateUserAsync(AppUser u)
        {
            return await AddUserAsync(u, false);
        }

        public bool ValidateTenantAgainstDomain(string domain, string tenantId)
        {
            //https://login.microsoftonline.com/feloniousmultitasking.com/federationmetadata/2007-06/federationmetadata.xml
            var fedmx = $"https://login.microsoftonline.com/{domain}/federationmetadata/2007-06/federationmetadata.xml";
            var c = new WebClient();
            try
            {
                var x = XDocument.Load(fedmx);
                var val = x.Element(XName.Get("EntityDescriptor", "urn:oasis:names:tc:SAML:2.0:metadata")).Attribute(XName.Get("entityID"));
                var fedTenant = Util.GetTenantIdFromIdPName(val.Value);
                Tc.TrackEvent("UserTenantValidation", new Dictionary<string, string> { { "Domain", domain }, { "TenantId", tenantId }, { "ResolvedTenantId", fedTenant } });
                return fedTenant == tenantId;
            }
            catch (Exception ex)
            {
                Tc.TrackException(ex);
                return false;
            }
        }
    }

    public class InstrumentedService
    {
        public TelemetryClient Tc { get; private set; }
        public InstrumentedService()
        {
            if (Tc == null) Tc = new TelemetryClient();
        }
    }
}