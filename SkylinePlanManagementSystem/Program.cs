using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using SkylinePlanManagementSystem.CustomerMiddlewares;
using SkylinePlanManagementSystem.DataRepositories;
using SkylinePlanManagementSystem.Infrastructure;
using SkylinePlanManagementSystem.Models;
using SkylinePlanManagementSystem.Security;
using NLog.Extensions.Logging;
using NLog.Web;
using System.Runtime;
using SkylinePlanManagementSystem.Security;
using SkylinePlanManagementSystem.Security.CustomTokenProvider;
using SkylinePlanManagementSystem.Infrastructure.Repositories;
using SkylinePlanManagementSystem.Application.Students;
using SkylinePlanManagementSystem.Application.Courses;
using SkylinePlanManagementSystem.Application.Teachers;
using SkylinePlanManagementSystem.Infrastructure.Data;
using NetCore.AutoRegisterDi;
using Microsoft.OpenApi.Models;
using System.Reflection;

namespace SkylinePlanManagementSystem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 清除默认的日志提供程序并使用 NLog
            //builder.Logging.ClearProviders();   // 清除默认的日志提供程序（可选）
            builder.Host.UseNLog();

            // 添加 MVC 服务并启用全局授权过滤器，要求所有控制器和操作方法都需要经过身份验证
            var mvcBuilder = builder.Services.AddControllersWithViews(config =>
            {
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();

                config.Filters.Add(new AuthorizeFilter(policy));
            })
            .AddXmlSerializerFormatters();
            // 只在开发环境开启 Razor 运行时编译
            if (builder.Environment.IsDevelopment())
            {
                mvcBuilder.AddRazorRuntimeCompilation();
            }

            // 自动注册服务（依赖注入）
            builder.Services.RegisterAssemblyPublicNonGenericClasses()
                .Where(c => c.Name.EndsWith("Service")) // 只注册以 "Service" 结尾的类
                .AsPublicImplementedInterfaces(ServiceLifetime.Scoped);   // 将它们注册为它们实现的接口
            builder.Services.AddSingleton<DataProtectionPurposeStrings>();
            builder.Services.AddTransient(typeof(IRepository<,>), typeof(RepositoryBase<,>));   // 注册泛型仓储服务

            // 注册 DbContext(MySQL 8)
            builder.Services.AddDbContextPool<AppDbContext>(options =>
                options
                //.UseLazyLoadingProxies()    // 启用延迟加载代理
                .UseMySql(
                    builder.Configuration.GetConnectionString("SkylinePlanDBConnection"),
                    ServerVersion.AutoDetect(
                        builder.Configuration.GetConnectionString("SkylinePlanDBConnection")
                    )
                ) 
            );

            // 注册 Identity 服务
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddErrorDescriber<CutsomIdentityErrorDescriber>()  // 使用自定义的错误描述器
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders()                         // 添加默认的令牌提供程序（用于生成密码重置令牌、电子邮件确认令牌等）
                .AddTokenProvider<CustomEmailConfirmationTokenProvider<ApplicationUser>>("CustomEmailConfirmation"); // 添加自定义的邮箱确认令牌提供程序，并指定名称

            // 配置 Identity 选项
            builder.Services.Configure<IdentityOptions>(options =>
            {
                // 密码相关设置
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 3;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;

                // 通过自定义的CustomEmailConfirmation名称来覆盖旧有token名称，
                // 是它与AddTokenProvider<CustomEmailConfirmationTokenProvider<ApplicationUser>>（"CustomEmailConfirmation")关联在一起
                options.Tokens.EmailConfirmationTokenProvider = "CustomEmailConfirmation"; // 使用自定义的邮箱确认令牌提供程序

                // 其它设置（如锁定、用户等）可以在这里配置
                options.SignIn.RequireConfirmedEmail = true; // 是否要求确认邮箱才能登录
                options.Lockout.MaxFailedAccessAttempts = 5; // 最大失败登录尝试次数
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15); // 锁定时间
            });

            // 修改所有令牌的默认有效期为5小时（包括密码重置令牌、邮箱确认令牌等）
            builder.Services.Configure<DataProtectionTokenProviderOptions>(o =>
            {
                o.TokenLifespan = TimeSpan.FromHours(5);
            });

            // 仅更改邮箱验证令牌的有效期为3天
            builder.Services.Configure<CustomEmailConfirmationTokenProviderOptions>(o =>
            {
                o.TokenLifespan = TimeSpan.FromDays(3);
            });

            // 配置 Identity 登录相关的 Cookie 行为
            builder.Services.ConfigureApplicationCookie(options =>
            {
                // 已登录但无权限时跳转的页面
                options.AccessDeniedPath = "/Admin/AccessDenied";
            });

            // 注册 HttpContextAccessor 服务 允许在自定义授权处理程序
            builder.Services.AddHttpContextAccessor();

            // 配置系统授权策略（基于角色和声明）
            builder.Services.AddAuthorization(options =>
            {
                // 基于角色的策略：只要属于指定角色之一即可访问
                options.AddPolicy("SuperAdminPolicy", policy =>
                    policy.RequireRole("Admin", "User", "SuperManager"));

                // 添加名为 "EditRolePolicy" 的授权策略
                options.AddPolicy("EditRolePolicy", policy =>
                {
                    // 向该策略中添加一个自定义授权需求（Requirement）
                    // Requirement 仅用于定义“需要满足的规则类型”
                    // 具体的判断逻辑在对应的 AuthorizationHandler 中实现
                    policy.AddRequirements(
                        new ManageAdminRolesAndClaimsRequirement());
                });

            });

            // 注册自定义授权处理程序（Handler）
            builder.Services.AddSingleton<IAuthorizationHandler,
                CanEditOnlyOtherAdminRolesAndClaimsHandler>();

            // 注册登录提供程序（Microsoft 和 GitHub）
            builder.Services
                .AddAuthentication()
                .AddMicrosoftAccount(microsoftOptions =>
                {
                    microsoftOptions.ClientId =
                        builder.Configuration["Authentication:Microsoft:ClientId"];

                    microsoftOptions.ClientSecret =
                        builder.Configuration["Authentication:Microsoft:ClientSecret"];
                })
                .AddGitHub(options =>
                {
                    options.ClientId = builder.Configuration["Authentication:GitHub:ClientId"];
                    options.ClientSecret = builder.Configuration["Authentication:GitHub:ClientSecret"];
                });

            // 注册 Swagger 生成器，定义一个或多个 Swagger 文件
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "SkylinePlanManagementSystem API",
                    Description = "为 SkylinePlanManagementSystem 系统，添加一个简单的 ASP.NET Core Web API 示例，由 李天喜 出品。",
                    Version = "v1",
                    Contact = new OpenApiContact
                    {
                        Name = "李天喜",
                        Email = "xsbnltx@163.com",
                        Url = new Uri("http://www.tanwanland.fun/"),
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Apache License 2.0",
                        Url = new Uri("https://www.apache.org/licenses/LICENSE-2.0.html"),
                    }
                });

                if (builder.Environment.IsDevelopment())
                {
                    // 设置Swagger JSON和UI的注释路径
                    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                    c.IncludeXmlComments(xmlPath);
                }
            });

            var app = builder.Build();

            // 判断环境生成开发异常页面或友好异常页面
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // 使用自定义的异常处理页面
                app.UseExceptionHandler("/Error");

                // 使用默认的状态码页面
                //app.UseStatusCodePages();

                // 使用自定义的状态码页面
                //app.UseStatusCodePagesWithRedirects("/Error/{0}");

                // 使用自定义的状态码页面，并保留原始请求的路径和查询字符串
                app.UseStatusCodePagesWithReExecute("/Error/{0}");
            }

            //app.MapGet("/", () => "Hello World!");

            app.UseDataInitializer();   // 启用数据初始化中间件，确保在应用启动时数据库中有初始数据

            app.UseStaticFiles();

            app.UseSwagger();       // 启用 Swagger 中间件
            app.UseSwaggerUI(c =>   // 启用 Swagger UI 中间件，它需要与 Swagger 配置在一起
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "SkylinePlanManagementSystem API V1");
            });

            app.UseAuthentication();// 启用身份验证中间件
            app.UseAuthorization(); // 启用授权中间件

            //app.UseRouting();       
            app.MapControllers();   // 启用属性路由   
            app.MapControllerRoute( // 启用默认路由
                name: "default",
                pattern: "{controller=Home}/{action=NewIndex}/{id?}");

            app.Run();
        }
    }
}
