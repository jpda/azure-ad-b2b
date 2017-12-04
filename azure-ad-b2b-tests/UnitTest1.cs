using System;
using System.Threading.Tasks;
using azure_ad_b2b_services;
using azure_ad_b2b_shared;
using Microsoft.Extensions.Options;
using Xunit;

namespace azure_ad_b2b_tests
{
    public class UnitTest1
    {
        private readonly IGraphService _graphService;
        public UnitTest1()
        {
            var opts = new GraphOptions()
            {
                Authority = "https://login.microsoftonline.com/oneclickcorporateespionage.com",
                ClientId = "12455dd5-8e0c-4ebc-b06d-88c49106a109",
                ClientKey = "BYalq55HNw6TU9Vvr1y/lj8jQXxKwA6/5ybXBJl28lg=",
                TenantName = "oneclickspy.onmicrosoft.com",
                TenantId = "92c39d6a-5d4d-462f-b03c-5593b750dbb8",
                Resource = "https://graph.microsoft.com/",
                AadResource = "https://graph.windows.net/",
                InviteRedirectUrl = "https://oneclickcorporateespionage.com/UserAdmin",
                AppObjectId = "6c70eb1a-0524-4782-87a5-06267b904ecf",
                CustomerAdminRoleId = "8d467ecd-c2f7-4dbe-bff8-a0b06b97faae",
                AppEnterpriseRegistrationResourceId = "652b995f-b131-4532-8eaa-e48f3258e71e"
            };
            IOptions<GraphOptions> graphConfig = Options.Create(opts);
            _graphService = new GraphService(graphConfig);
        }
        [Fact]
        public async Task Test1()
        {
            var user = await _graphService.InviteUser("joe2@feloniousmultitasking.com");
            Console.WriteLine(user.InvitedUserId);
        }

        [Fact]
        public async Task TestAddUserToRole()
        {
            var userID = "c06f897b-c591-4df3-8e2f-c4c75e03461b";
            await _graphService.AddUserToRole(userID, true);
        }
    }
}
