namespace azure_ad_b2b_entities.AadTenant
{
    public class AadUserEntity : AadEntity
    {
        public string NameIdentifier { get; set; }
        public string Upn { get; set; }

        public AadUserEntity() { }

        public AadUserEntity(string tenant, string nameIdentifier, string upn) : base(tenant)
        {
            NameIdentifier = nameIdentifier;
            Upn = upn;
            RowKey = Upn;
        }
    }
}