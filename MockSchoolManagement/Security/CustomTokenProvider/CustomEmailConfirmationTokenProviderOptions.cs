using Microsoft.AspNetCore.Identity;

namespace MockSchoolManagement.Security.CustomTokenProvider
{
    /// <summary>
    /// 自定义邮箱验证令牌有效期配置类
    /// </summary>
    public class CustomEmailConfirmationTokenProviderOptions: DataProtectionTokenProviderOptions
    {
    }
}
