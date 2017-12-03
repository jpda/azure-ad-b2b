using System;
using System.Collections.Generic;

namespace azure_ad_b2b_shared
{
    public static class Util
    {
        private const string IDENTITY_PROVIDER_CLAIM_TYPE_NAME = "http://schemas.microsoft.com/identity/claims/identityprovider";

        public static string GetTenantIdFromIdPName(string issuer)
        {
            if (Uri.TryCreate(issuer, UriKind.Absolute, out Uri issuerUri))
            {
                var authorityHost = issuerUri.DnsSafeHost.ToLower();
                if (authorityHost.Contains("sts.windows.net") || authorityHost.Contains("microsoftonline.")) // AAD
                {
                    return issuerUri.PathAndQuery.TrimEnd('/').TrimStart('/');
                }
                return issuerUri.ToString();
            }
            return issuer;
        }
    }
}