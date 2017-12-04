using System.Threading.Tasks;

namespace azure_ad_b2b_services
{
    public interface IGraphService
    {
        Task<InviteUserGraphResponse> InviteUser(string target, bool sendEmail = false, string displayName = "");
        Task<bool> AddUserToRole(string userIdentifier, bool isCustomerAdmin);
    }

    public struct InviteUserGraphResponse
    {
        public string InvitedUserId { get; set; }
        public string InvitedUserInviteRedeemUrl { get; set; }
    }
}