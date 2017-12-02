using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace azure_ad_b2b_services
{
    public class GraphService : IGraphService
    {
        GraphConfiguration _config;
        public GraphService(IOptions<GraphConfiguration> config)
        {
            _config = config.Value;
        }

        public async Task<string> InviteUser(string target, bool sendEmail = false, string displayName = "")
        {
            var ctx = new AuthenticationContext(_config.Authority);
            var token = await ctx.AcquireTokenAsync(_config.Resource, new ClientCredential(_config.ClientId, _config.ClientKey));

            var graphserviceClient = new GraphServiceClient(
                   new DelegateAuthenticationProvider(
                       (requestMessage) =>
                       {
                           requestMessage.Headers.Authorization = new AuthenticationHeaderValue("bearer", token.AccessToken);

                           return Task.FromResult(0);
                       }));

            var invite = new Invitation()
            {
                InvitedUserDisplayName = displayName,
                InvitedUserEmailAddress = target,
                InviteRedirectUrl = _config.InviteRedirectUrl,
                SendInvitationMessage = sendEmail
            };
            var inviteResponse = await graphserviceClient.Invitations.Request().AddAsync(invite);
            return inviteResponse.InviteRedeemUrl;
        }
    }
}