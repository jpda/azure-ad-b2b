using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using azure_ad_b2b_shared;
using AAD = Microsoft.Azure.ActiveDirectory.GraphClient;
using System;
using System.Linq;
using System.Diagnostics.Contracts;

namespace azure_ad_b2b_services
{
    public class GraphService : IGraphService
    {
        private readonly GraphOptions _config;
        private readonly GraphServiceClient _client;
        private readonly AAD.ActiveDirectoryClient _aadClient;

        public GraphService(IOptions<GraphOptions> config)
        {
            _config = config.Value;
            _client = GetServiceClient().Result;
            _aadClient = GetAadServiceClient();
        }

        private async Task<GraphServiceClient> GetServiceClient()
        {
            var token = await GetAccessToken(_config.Resource);

            var graphserviceClient = new GraphServiceClient(
                   new DelegateAuthenticationProvider(
                       (requestMessage) =>
                       {
                           requestMessage.Headers.Authorization = new AuthenticationHeaderValue("bearer", token);
                           return Task.FromResult(0);
                       }));
            return graphserviceClient;
        }

        private AAD.ActiveDirectoryClient GetAadServiceClient()
        {
            var root = new Uri(_config.AadResource);
            var tenantRoot = new Uri(root, _config.TenantId);
            var client = new AAD.ActiveDirectoryClient(tenantRoot, async () => await GetAccessToken(_config.AadResource));
            return client;
        }

        private async Task<string> GetAccessToken(string resource)
        {
            var ctx = new AuthenticationContext(_config.Authority);
            var token = await ctx.AcquireTokenAsync(resource, new ClientCredential(_config.ClientId, _config.ClientKey));
            return token.AccessToken;
        }

        public async Task<string> InviteUser(string target, bool sendEmail = false, string displayName = "")
        {
            var invite = new Invitation()
            {
                InvitedUserDisplayName = displayName,
                InvitedUserEmailAddress = target,
                InviteRedirectUrl = _config.InviteRedirectUrl,
                SendInvitationMessage = sendEmail
            };
            var inviteResponse = await _client.Invitations.Request().AddAsync(invite);
            return inviteResponse.InviteRedeemUrl;
        }

        // see: https://developer.microsoft.com/en-us/graph/docs/api-reference/beta/api/approleassignment_update for Microsoft Graph reference
        // this uses the AAD Graph since it appears to be the only place it is implemented currently (12/2017)
        public async Task AddUserToRole(string userIdentifier)
        {
            var appRef = _aadClient.Applications.GetByObjectId(_config.AppObjectId);
            var app = (AAD.Application)appRef.ToApplication();
            var servicePrincipal = (AAD.ServicePrincipal)appRef.ToServicePrincipal();
            var user = (AAD.User)(await _aadClient.Users.Where(x => x.UserPrincipalName == userIdentifier).ExecuteSingleAsync());
            if (app.ObjectId != null && user != null && servicePrincipal.ObjectId != null)
            {
                var appRoleAssignment = new AAD.AppRoleAssignment
                {
                    Id = app.AppRoles.FirstOrDefault().Id,
                    ResourceId = Guid.Parse(servicePrincipal.ObjectId),
                    PrincipalType = "User",
                    PrincipalId = Guid.Parse(user.ObjectId)
                };
                user.AppRoleAssignments.Add(appRoleAssignment);
                await user.UpdateAsync();
            }
        }
    }
}