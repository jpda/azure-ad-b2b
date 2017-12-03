using System;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using azure_ad_b2b_shared;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Threading.Tasks;
using System.Linq;
using System.Security.Claims;

namespace Microsoft.AspNetCore.Authentication
{
    public static class AzureAdAuthenticationBuilderExtensions
    {
        public static AuthenticationBuilder AddAzureAd(this AuthenticationBuilder builder)
            => builder.AddAzureAd(_ => { });

        public static AuthenticationBuilder AddAzureAd(this AuthenticationBuilder builder, Action<AzureAdOptions> configureOptions)
        {
            builder.Services.Configure(configureOptions);
            builder.Services.AddSingleton<IConfigureOptions<OpenIdConnectOptions>, ConfigureAzureOptions>();
            builder.AddOpenIdConnect(opts =>
            {
                //opts.ResponseType = "code id_token";
                opts.Events = new OpenIdConnectEvents()
                {
                    //OnAuthorizationCodeReceived = async ctx =>
                    //{
                    //    var cred = new ClientCredential(ctx.Options.ClientId, ctx.Options.ClientSecret);
                    //    var authctx = new AuthenticationContext(ctx.Options.Authority);
                    //    var token = await authctx.AcquireTokenByAuthorizationCodeAsync(ctx.TokenEndpointRequest.Code, new Uri(ctx.TokenEndpointRequest.RedirectUri), cred, ctx.Options.Resource);
                    //    ctx.HandleCodeRedemption(token.AccessToken, ctx.ProtocolMessage.IdToken);
                    //}
                    OnTokenValidated = ctx =>
                    {
                        var idpClaimTypeName = "http://schemas.microsoft.com/identity/claims/identityprovider";
                        if (!ctx.Principal.HasClaim(x => x.Type == idpClaimTypeName)) return Task.CompletedTask;

                        var identityProvider = ctx.Principal.Claims.Single(x => x.Type == idpClaimTypeName).Value;
                        var homeTenant = Util.GetTenantIdFromIdPName(identityProvider);
                        var i = new ClaimsIdentity();
                        i.AddClaim(new Claim("http://schemas.jpd.ms/aad/tenantId", homeTenant));
                        ctx.Principal.AddIdentity(i);
                        return Task.CompletedTask;
                    }
                };
            });
            return builder;
        }

        private class ConfigureAzureOptions : IConfigureNamedOptions<OpenIdConnectOptions>
        {
            private readonly AzureAdOptions _azureOptions;

            public ConfigureAzureOptions(IOptions<AzureAdOptions> azureOptions)
            {
                _azureOptions = azureOptions.Value;
            }

            public void Configure(string name, OpenIdConnectOptions options)
            {
                options.ClientId = _azureOptions.ClientId;
                options.Authority = $"{_azureOptions.Instance}{_azureOptions.TenantId}";
                options.UseTokenLifetime = true;
                options.CallbackPath = _azureOptions.CallbackPath;
                options.RequireHttpsMetadata = false;
            }

            public void Configure(OpenIdConnectOptions options)
            {
                Configure(Options.DefaultName, options);
            }
        }
    }
}
