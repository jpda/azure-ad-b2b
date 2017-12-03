using azure_ad_b2b_entities.AadTenant;

namespace azure_ad_b2b_services.AadTenantRepo
{
    public class AadTenant
    {
        public string TenantId { get; set; }
        public string TenantUrl { get; set; }
        public string Name { get; set; }
        public string Issuer { get; set; }
        public bool AdminConsent { get; set; }

        public AadTenant(AadTenantEntity e)
        {
            TenantId = e.TenantId;
            TenantUrl = e.TenantUrl;
            AdminConsent = e.AdminConsent;
            Issuer = e.SigningIdentifier;
            Name = e.Name;
        }

        public AadTenant()
        {

        }
    }
}
