using azure_ad_b2b_services.AppTenantRepo;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace azure_ad_b2b_services
{
    public interface IGraphService
    {
        Task<string> InviteUser(string target, bool sendEmail, string displayName);
        Task AddUserToRole(string userIdentifier);
    }
}