using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace azure_ad_b2b_entities.AppTenant
{
    public class AppUserEntity : TableEntity
    {
        public string TenantId { get; set; }
        public string Upn { get; set; }
        public string NameIdentifier { get; set; }
        public string AddedBy { get; set; }
        public DateTime DateAdded { get; set; }
        public string InviteRedeemUrl { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }

        public AppUserEntity(string tenantId, string email) : this()
        {
            TenantId = tenantId;
            Email = email;
            PartitionKey = tenantId;
            RowKey = email;
        }

        public AppUserEntity() : base()
        {
        }
    }
}
