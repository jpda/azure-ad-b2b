using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace azure_ad_b2b_entities.AadTenant
{
    public abstract class AadEntity : TableEntity
    {
        /// <summary>
        /// don't use this property directly, for table storage compat
        /// </summary>
        public string EncodedTenantUrl { get; set; }

        //tenant is generally a URL, so we should encode/decode it on it's way in/out
        [IgnoreProperty]
        public string TenantUrl
        {
            get => System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(EncodedTenantUrl));
            set => EncodedTenantUrl = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(value));
        }

        public string TenantId { get; set; }

        public string Name { get; set; }

        protected AadEntity()
        {
            if (PartitionKey == null)
            {
                PartitionKey = GetType().Name;
            }
        }


        protected AadEntity(string tenantId) : this()
        {
            TenantId = tenantId;
        }
    }
}