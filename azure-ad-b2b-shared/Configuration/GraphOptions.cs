namespace azure_ad_b2b_shared
{
    public class GraphOptions
    {
        public string ClientId { get; set; }
        public string ClientKey { get; set; }
        public string Authority { get; set; }
        public string Resource { get; set; }
        public string InviteRedirectUrl { get; set; } 
    }
}