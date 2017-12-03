using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace azure_ad_b2b_entities.AppTenant
{
    public class AppTenantEntity : AppEntity
    {
        public string TenantId { get; set; }
        public string Name { get; set; }
        public string AdminEmail { get; set; }
        public DateTime DateAdded { get; set; }
        public bool InviteSent { get; set; }
        public string InvitedBy { get; set; }
        public string InviteRedeemUrl { get; set; }

        public AppTenantEntity() { }
        public AppTenantEntity(string tenantId) : base()
        {
            TenantId = tenantId;
            RowKey = tenantId;
        }
        public AppTenantEntity(string tenantId, string name, string admin, string inviter, string inviteRedeemUrl) : this(tenantId)
        {
            TenantId = tenantId;
            Name = name;
            AdminEmail = admin;
            DateAdded = DateTime.UtcNow;
            InviteSent = true;
            InvitedBy = inviter;
            InviteRedeemUrl = inviteRedeemUrl;
        }
    }

    public class AppEntity : TableEntity
    {
        public AppEntity()
        {
            if (string.IsNullOrEmpty(PartitionKey))
            {
                PartitionKey = GetType().Name;
            }
        }
    }
}
