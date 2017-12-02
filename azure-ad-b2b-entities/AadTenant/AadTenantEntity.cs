using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace azure_ad_b2b_entities.AadTenant
{
    public class AadTenantEntity : AadEntity
    {
        public string SigningIdentifier { get; set; }
        public bool AdminConsent { get; set; }

        public AadTenantEntity() { }

        public AadTenantEntity(string tenantUrl) : base(tenantUrl) { RowKey = TenantUrl; }

        public AadTenantEntity(string tenantId, string signer, bool adminConsent = false) : this(tenantId)
        {
            SigningIdentifier = signer;
            AdminConsent = adminConsent;
        }
    }
}
