# SkylinePlanManagementSystem

一个基于 **ASP.NET Core 8 MVC + EF Core + MySQL + Identity** 的项目管理与教学管理示例系统。

> 当前仓库包含部门、项目计划（含节点与子节点进度）等模块，并集成了登录认证、角色授权、Swagger、NLog 日志与后台任务。

## ✨ 功能概览

- 用户与权限
  - ASP.NET Core Identity 本地认证
  - 邮箱确认、密码重置、账号锁定
  - 角色/声明策略授权（如 `SuperAdminPolicy`、`EditRolePolicy`）
  - 第三方登录：Microsoft、GitHub
- 业务模块
  - 项目计划管理（Project / Node / SubNode）
  - 项目进度后台服务自动更新（HostedService）
- 工程能力
  - Swagger/OpenAPI 文档
  - NLog 日志
  - 分层结构（Application / Infrastructure / Controllers / Views）

## 🧱 技术栈

- .NET 8 (`net8.0`)
- ASP.NET Core MVC
- Entity Framework Core 8 + Pomelo MySQL Provider
- ASP.NET Core Identity
- Swashbuckle (Swagger)
- NLog

## 📁 项目结构（核心）

```text
SkylinePlanManagementSystem/
├─ Application/              # 应用服务层（Courses/Students/Teachers/Projects...）
├─ Controllers/              # MVC 控制器
├─ Infrastructure/           # DbContext、仓储、数据初始化、实体映射
├─ Models/                   # 领域模型/实体
├─ ViewModels/               # 视图模型
├─ Views/                    # Razor 视图
├─ wwwroot/                  # 静态资源
├─ Program.cs                # 应用入口与依赖注入配置
└─ appsettings*.json         # 配置文件
```

## 🚀 快速开始

### 1) 环境准备

- .NET SDK 8.0+
- MySQL 8.0+
- （可选）Visual Studio 2022 / VS Code + C# 插件

### 2) 获取代码

```bash
git clone <your-repo-url>
cd SkylinePlanManagementSystem
```

### 3) 配置数据库连接

编辑 `SkylinePlanManagementSystem/appsettings.json` 中的连接字符串：

```json
"ConnectionStrings": {
  "SkylinePlanDBConnection": "server=localhost;port=3306;database=SPMSdb;user=<user>;password=<password>;"
}
```

### 4) 执行迁移并更新数据库

```bash
dotnet ef database update --project SkylinePlanManagementSystem
```

> 如果本地没有安装 `dotnet-ef`，可先执行：`dotnet tool install --global dotnet-ef`

### 5) 运行项目

```bash
dotnet run --project SkylinePlanManagementSystem
```

默认路由：

- 首页：`/Home/NewIndex`
- Swagger：`/swagger`

## 🔐 配置说明

项目使用以下关键配置：

- `ConnectionStrings:SkylinePlanDBConnection`：MySQL 连接
- `Authentication:Microsoft:*`：Microsoft OAuth
- `Authentication:GitHub:*`：GitHub OAuth

建议在本地开发中使用 **User Secrets** 或环境变量管理敏感信息，不要将真实密钥提交到仓库。

## 🧪 常用命令

```bash
# 还原依赖
dotnet restore SkylinePlanManagementSystem/SkylinePlanManagementSystem.csproj

# 编译
dotnet build SkylinePlanManagementSystem/SkylinePlanManagementSystem.csproj

# 运行
dotnet run --project SkylinePlanManagementSystem
```

## 📌 开发说明

- 默认启用了全局授权过滤器（RequireAuthenticatedUser），大多数页面需要登录访问。
- 开发环境下启用 Razor Runtime Compilation，便于视图热更新。
- 项目启动时会运行数据初始化逻辑（`UseDataInitializer()`）。

## 📝 License

本项目仅为学习参考，如有不妥请联系。
