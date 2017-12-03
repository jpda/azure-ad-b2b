using System;
using System.Collections.Generic;
using System.Text;
using azure_ad_b2b_entities;
using azure_ad_b2b_entities.AadTenant;

namespace azure_ad_b2b_services.AadTenantRepo
{

    public class AadUser
    {
        public string TenantId { get; set; }
        public string Upn { get; set; }
        public string NameIdentifier { get; set; }

        public AadUser(AadUserEntity dataEntity)
        {
            TenantId = dataEntity.TenantId;
            Upn = dataEntity.Upn;
            NameIdentifier = dataEntity.NameIdentifier;
        }

        public AadUser()
        {

        }
    }
}
