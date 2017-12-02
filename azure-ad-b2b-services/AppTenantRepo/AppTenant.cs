using azure_ad_b2b_entities.AppTenant;
using System;

namespace azure_ad_b2b_services.AppTenantRepo
{
    public class AppTenant
    {
        public string TenantId { get; set; }
        public string Name { get; set; }
        public string AdminEmail { get; set; }
        public DateTime DateAdded { get; set; }
        public bool InviteSent { get; set; }
        public string InvitedBy { get; set; }

        public AppTenant(AppTenantEntity e)
        {
            TenantId = e.TenantId;
            Name = e.Name;
            AdminEmail = e.AdminEmail;
            DateAdded = e.DateAdded;
            InviteSent = e.InviteSent;
            InvitedBy = e.InvitedBy;
        }

        public AppTenant()
        {

        }
    }
}
