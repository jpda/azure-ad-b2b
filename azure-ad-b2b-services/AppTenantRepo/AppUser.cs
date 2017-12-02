using System;
using azure_ad_b2b_entities.AppTenant;

namespace azure_ad_b2b_services.AppTenantRepo
{

    public class AppUser
    {
        public string TenantId { get; set; }
        public string Upn { get; set; }
        public string NameIdentifier { get; set; }
        public string AddedBy { get; set; }
        public DateTime DateAdded { get; set; }
        public string InviteRedeemUrl { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }

        public AppUser(AppUserEntity dataEntity)
        {
            TenantId = dataEntity.TenantId;
            Email = dataEntity.Email;
            DisplayName = dataEntity.DisplayName;
            Upn = dataEntity.Upn;
            NameIdentifier = dataEntity.NameIdentifier;
            AddedBy = dataEntity.AddedBy;
            DateAdded = dataEntity.DateAdded;
            InviteRedeemUrl = dataEntity.InviteRedeemUrl;
        }

        public AppUser()
        {

        }
    }
}
