namespace azure_ad_b2b_shared
{
    public class GraphOptions
    {
        public string ClientId { get; set; }
        public string ClientKey { get; set; }
        public string Authority { get; set; }
        public string TenantName { get; set; }
        public string TenantId { get; set; }
        public string Resource { get; set; }
        public string AadResource { get; set; }
        public string InviteRedirectUrl { get; set; }
        public string AppObjectId { get; set; }
        public string CustomerAdminRoleId { get; set; }
        public string CustomerUserRoleId { get; set; }
        public string AppEnterpriseRegistrationResourceId { get; set; }
    }
}