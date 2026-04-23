using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace SkylinePlanManagementSystem.Security.CustomTokenProvider
{
    /// <summary>
    /// 自定义邮箱验证令牌提供程序
    /// </summary>
    /// <typeparam> name="TUser"</typeparam>
    public class CustomEmailConfirmationTokenProvider<TUser> : DataProtectorTokenProvider<TUser> where TUser : class
    {
        public CustomEmailConfirmationTokenProvider(IDataProtectionProvider dataProtectionProvider,
            IOptions<CustomEmailConfirmationTokenProviderOptions> options, 
            ILogger<DataProtectorTokenProvider<TUser>> logger): base(dataProtectionProvider, options, logger)
        {
        }
    }
}
