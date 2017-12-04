using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using azure_ad_b2b_shared;
using AAD = Microsoft.Azure.ActiveDirectory.GraphClient;
using System;
using System.Linq;
using System.Net.Http;

namespace azure_ad_b2b_services
{
    public class GraphService : IGraphService
    {
        private readonly GraphOptions _config;
        private readonly GraphServiceClient _client;
        private readonly AAD.ActiveDirectoryClient _aadClient;
        private string _token;
        private string _aadToken;

        public GraphService(IOptions<GraphOptions> config)
        {
            _config = config.Value;
            _client = GetServiceClient().Result;
            _aadClient = GetAadServiceClient();
            _aadToken = GetAccessToken(_config.AadResource).Result;
        }

        private async Task<GraphServiceClient> GetServiceClient()
        {
            var token = await GetAccessToken(_config.Resource);
            _token = token;

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

        public async Task<InviteUserGraphResponse> InviteUser(string target, bool sendEmail = false, string displayName = "")
        {
            var invite = new Invitation()
            {
                InvitedUserDisplayName = displayName,
                InvitedUserEmailAddress = target,
                InviteRedirectUrl = _config.InviteRedirectUrl,
                SendInvitationMessage = sendEmail
            };
            var inviteResponse = await _client.Invitations.Request().AddAsync(invite);
            return new InviteUserGraphResponse() { InvitedUserId = inviteResponse.InvitedUser.Id, InvitedUserInviteRedeemUrl = inviteResponse.InviteRedeemUrl };
        }

        public async Task<bool> AddUserToRole(string userId)
        {
            var content = $@"{{'id': '{_config.CustomerAdminRoleId}', 'principalId': '{userId}','resourceId': '{_config.AppEnterpriseRegistrationResourceId}'}}";
            var c = new HttpClient();
            //https://graph.windows.net/oneclickspy.onmicrosoft.com/users/c06f897b-c591-4df3-8e2f-c4c75e03461b/appRoleAssignments?api-version=1.6
            c.DefaultRequestHeaders.Clear();
            c.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _aadToken);
            c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var targetUri = new Uri($"https://graph.windows.net/{_config.TenantId}/users/{userId}/appRoleAssignments?api-version=1.6");
            var response = await c.PostAsync(targetUri, new StringContent(content, System.Text.Encoding.UTF8, "application/json"));
            System.Diagnostics.Debug.WriteLine(await response.Content.ReadAsStringAsync());
            return true;
        }

        // see: https://developer.microsoft.com/en-us/graph/docs/api-reference/beta/api/approleassignment_update for Microsoft Graph reference
        // this uses the AAD Graph since it appears to be the only place it is implemented currently (12/2017)
        public async Task<bool> AddUserToRole2(string userIdentifier)
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
                return true;
            }
            return false;
        }
    }

    public static class Extensions
    {
        public static async Task<HttpResponseMessage> PatchAsync(this HttpClient client, Uri requestUri, StringContent content)
        {
            var method = new HttpMethod("PATCH");
            var req = new HttpRequestMessage(method, requestUri);
            req.Content = content;
            var response = new HttpResponseMessage();
            try
            {
                response = await client.SendAsync(req);
            }
            catch (HttpRequestException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            return response;
        }
    }
}