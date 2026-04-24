using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SkylinePlanManagementSystem.Models;
using SkylinePlanManagementSystem.Infrastructure.Repositories;
using SkylinePlanManagementSystem.ViewModels.Account;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SkylinePlanManagementSystem.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private UserManager<ApplicationUser> _userManager;
        private SignInManager<ApplicationUser> _signInManager;
        private readonly IRepository<Department, int> _departmentRepository;
        private readonly ILogger<AdminController> _logger;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IRepository<Department, int> departmentRepository,
            ILogger<AdminController> logger)
        {
            this._userManager = userManager;
            this._signInManager = signInManager;
            _departmentRepository = departmentRepository;
            _logger = logger;
        }

        private SelectList DepartmentsDropDownList(object selectedDepartment = null)
        {
            var departments = _departmentRepository.GetAll().OrderBy(a => a.Name).ToList();
            return new SelectList(departments, "DepartmentId", "Name", selectedDepartment);
        }

        [HttpGet]
        public async Task<IActionResult> AddPassword()
        {
            var user = await _userManager.GetUserAsync(User);

            var userHasPassword = await _userManager.HasPasswordAsync(user);
            if(userHasPassword)
            {
                return RedirectToAction("ChangePassword");
            }
            
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddPassword(AddPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync (User);
                // 为用户添加密码。
                var result = await _userManager.AddPasswordAsync(user, model.NewPassword);
                if (!result.Succeeded)
                {
                    foreach(var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View();
                }
                // 刷新当前用户的Cookie以反映密码更改
                await _signInManager.RefreshSignInAsync(user);
                return View("AddPasswordConfirmation");
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {
            var user = await _userManager.GetUserAsync(User);

            // 判断当前用户是否拥有密码，如果没有密码，则重定向到AddPassword视图中
            var userHasPassword = await _userManager.HasPasswordAsync(user);

            if (!userHasPassword)
            {
                return RedirectToAction("AddPassword");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if(user == null)
                {
                    return RedirectToAction("Login");
                }

                // 使用ChangePasswordAsync方法更改用户密码
                var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

                // 如果新密码不符合复杂性规则或当前密码不正确，则将错误提示返回到ChangePassword视图中
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View();
                }
                // 更改密码成功，会刷新登录Cookie
                await _signInManager.RefreshSignInAsync(user);
                return View("ChangePasswordConfirmation");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult ResetPassword(string token, string email)
        {
            // 如果密码重置令牌或邮箱地址无效，则可能是用户在试图篡改密码重置链接
            if (token == null || email == null)
            {
                ModelState.AddModelError("", "无效的密码重置令牌");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                // 通过邮箱地址查找用户
                var user = await _userManager.FindByEmailAsync(model.Email);
                
                if(user != null)
                {
                    // 重置用户密码
                    var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
                    if(result.Succeeded)
                    {
                        // 密码重置成功后，如果当前账户被锁定，则设置该账户锁定结束时间为当前UTC日期时间，这样就解锁了账户
                        if(await _userManager.IsLockedOutAsync(user))
                        {
                            await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow);
                            // DateTimeOffset指的是UTC日期时间即格林威治时间。
                        }
                        return View("ResetPasswordConfirmation");
                    }

                    // 显示验证错误信息。当密码重置令牌已用或密码复杂性要求未满足时，触发行为
                    foreach(var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }

                    return View(model);
                }

                // 为避免暴力攻击，不进行用户不存在的提示
                return View("ResetPasswordConfirmation");
            }

            // 如果模型验证失败，则返回视图，并显示验证错误信息
            return View(model);
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(EmailAddressViewModel model)
        {
            if (ModelState.IsValid)
            {
                // 通过邮箱地址查询用户地址
                var user = await _userManager.FindByEmailAsync(model.Email);

                // 如果找到了用户并且确认了电子邮箱
                if(user != null && await _userManager.IsEmailConfirmedAsync(user))
                {
                    // 生成重置密码令牌
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    // 生成重置密码链接
                    var passwordResetLink = Url.Action("ResetPassword", "Account", new { email = model.Email, token = token }, Request.Scheme);
                    _logger.Log(LogLevel.Warning, passwordResetLink);

                    // 重定向到忘记密码确认视图
                    return View("ForgotPasswordConfirmation");
                }
                // 为避免暴力攻击，不进行用户不存在或邮箱未验证的提示
                return View("ForgotPasswordConfirmation");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult ActivateUserEmail()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ActivateUserEmail(EmailAddressViewModel model)
        {
            if (ModelState.IsValid)
            {
                // 通过邮箱地址重新用户地址
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user != null)
                {
                    if (!await _userManager.IsEmailConfirmedAsync(user))
                    {
                        // 生成电子邮件确认令牌
                        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        // 生成电子邮件确认链接
                        var confirmationLink = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, token = token }, Request.Scheme);
                        _logger.Log(LogLevel.Warning, confirmationLink);
                        ViewBag.Message = "如果您在我们系统有注册账户，我们已经给您发了一份邮件，请前往邮箱激活您账户。";
                        // 重定向到忘记邮箱确认视图
                        return View("ActivateUserEmailConfirmation", ViewBag.Message);
                    }
                }
            }
            ViewBag.Message = "请确认邮箱是否存在异常，现在我们无法给您发送激活链接";
            // 为避免暴力攻击，不进行用户不存在或邮箱未验证的提示
            return View("ActivateUserEmailConfirmation", ViewBag.Message);
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if(userId == null || token == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var user = await _userManager.FindByIdAsync(userId);

            if(user == null)
            {
                ViewBag.ErrorMessage = $"当前 {userId} 无效";
                return View("NotFound");
            }
            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (result.Succeeded)
            {
                return View();
            }

            ViewBag.ErrorTitle = "您的电子邮箱还未进行验证";
            return View("Error");
        }

        [AcceptVerbs("Get", "Post")]
        [AllowAnonymous]
        public async Task<IActionResult> IsEmailInUse(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if(user == null)
            {
                return Json(true);
            }
            else
            {
                return Json($"邮箱 {email} 已被注册使用了");
            }
        }

        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            LoginViewModel loginViewModel = new LoginViewModel()
            {
                ReturnUrl = returnUrl,
                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };

            if(remoteError != null)
            {
                ModelState.AddModelError(string.Empty, $"第三方登录提供程序错误: {remoteError}");
                return View("Login", loginViewModel);
            }

            // 从第三方登录程序提供商，获取关于用户的登录信息
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ModelState.AddModelError(string.Empty, "加载第三方登录信息失败");
                return View("Login", loginViewModel);       
            }

            // 获取邮箱地址
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            ApplicationUser user = null;

            if(email != null)
            {
                // 通过邮箱地址查询用户是否已存在
                user = await _userManager.FindByEmailAsync(email);

                // 如果邮箱未确认，返回登录视图，并显示错误信息
                if(user != null && !user.EmailConfirmed)
                {
                    ModelState.AddModelError(string.Empty, "您的电子邮箱还未进行验证");
                    return View("Login", loginViewModel);
                }
            }

            // 如果之前已经登录过了，则会在AspNetUserLogins表中有记录，这时无需创建新记录，直接登录即可
            var sigInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (sigInResult.Succeeded)
            {
                return LocalRedirect(returnUrl);
            }
            // 如果AspNetUserLogins表中没有记录，则代表用户没有一个本地账户，此时需要创建一个记录
            else
            {
                if (email != null)
                {
                    if (user == null)
                    {
                        user = new ApplicationUser
                        {
                            UserName = info.Principal.FindFirstValue(ClaimTypes.Email),
                            Email = info.Principal.FindFirstValue(ClaimTypes.Email),
                            Name = info.Principal.FindFirstValue(ClaimTypes.Name) ?? "外部用户",
                            DepartmentId = null
                        };
                        // 如果不存用户，则创建一个新用户，并将其存储在AspNetUsers数据库表中
                        await _userManager.CreateAsync(user);

                        // 生成电子邮件确认令牌
                        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        // 生成电子邮件确认链接
                        var confirmationLink = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, token = token }, Request.Scheme);
                        _logger.Log(LogLevel.Warning, confirmationLink);
                        ViewBag.ErrorTitle = "注册成功";
                        ViewBag.ErrorMessage = "在您登入系统前，我们已经给您发了一份邮件，需要您先进行邮件验证，单击确认链接即可完成";
                        return View("Error");
                    }

                    // 在AspNetUserLogins表中创建一条记录，然后将挡当前用户登录到系统中
                    await _userManager.AddLoginAsync(user, info);
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    return LocalRedirect(returnUrl);
                }

                // 如果获取不到电子邮件地址，则需要将请求重定向到错误视图中
                ViewBag.ErrorTitle = $"无法从提供商 {info.LoginProvider} 获取用户的邮箱地址";
                ViewBag.ErrorMessage = "请联系管理员，获取更多帮助。";

                return View("Error");
            }
        }

        [HttpPost]
        public IActionResult ExternalLogin(string provider, string returnUrl)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl)
        {
            LoginViewModel model = new LoginViewModel()
            {
                ReturnUrl = returnUrl,
                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl)
        {
            model.ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.UserName);
                if(user != null && !user.EmailConfirmed && (await _userManager.CheckPasswordAsync(user, model.Password)))
                {
                    ModelState.AddModelError(string.Empty, "您的电子邮箱还未进行验证。");
                    return View(model);
                }

                // 在PasswordSignInAsync()中将最后一个参数从false改为true，以启用账户锁定功能。
                // 每次登录失败后，都会将AspNetUsers表中的AccessFailedCount字段加1，当连续登录失败次数达到指定值时，
                // MaxFailedAccessAttempts将会锁定账户，然后修改LockoutEnd字段，添加解锁时间
                // 即使提供了正确的密码，PasswordSignInAsync()方法返回的值依然是LocakedOut
                var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, true);
                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(returnUrl))
                    {
                        if (Url.IsLocalUrl(returnUrl))
                        {
                            return Redirect(returnUrl);
                        }
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }

                // 如果账户状态为LockedOut，则重定向到AccountLocked视图中,提示用户账户已被锁定
                if (result.IsLockedOut)
                {
                    return View("AccountLocked");
                }

                ModelState.AddModelError(string.Empty, "登陆失败，请重试");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            var model = new RegisterViewModel
            {
                DepartmentList = DepartmentsDropDownList()
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            model.DepartmentList = DepartmentsDropDownList(model.DepartmentId);
            if(ModelState.IsValid)
            {
                // 将数据从RegisterViewModel复制到IdentityUser
                var user = new ApplicationUser
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    Name = model.Name,
                    PhoneNumber = model.PhoneNumber,
                    DepartmentId = model.DepartmentId
                };

                // 将用户存储在AspNetUsers数据库表中
                var result = await _userManager.CreateAsync(user, model.Password);

                // 如果创建用户成功，则使用登录服务SignInManager进行登录
                // 然后重定向到Home控制器的Index方法
                if (result.Succeeded)
                {
                    // 生成电子邮件确认令牌
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    // 生成电子邮件确认链接
                    var confirmationLink = Url.Action("ConfirmEmail", "Account", new {userId = user.Id, token = token }, Request.Scheme);

                    // 需注入ILogger<AccountController> _logger;服务，记录生成的URL链接到日志中，方便调试使用
                    _logger.Log(LogLevel.Warning, confirmationLink);

                    // 如果用户已登录且角色为Admin，就重定向到ListUsers视图中
                    if (_signInManager.IsSignedIn(User) && User.IsInRole("Admin"))
                    {
                        return RedirectToAction("ListUsers", "Admin");
                    }

                    ViewBag.ErrorTitle = "注册成功";
                    ViewBag.ErrorMessage = "在您登入系统前，我们已经给您发了一份邮件，需要您先进行邮件验证，单击确认链接即可完成";
                    return View("Error");
                }

                // 如果创建用户失败，则将错误信息添加到ModelState对象中
                // 将由验证摘要标记助手显示在视图中
                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            var model = new UserProfileViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Name = user.Name,
                PhoneNumber = user.PhoneNumber,
                DepartmentId = user.DepartmentId,
                DepartmentList = DepartmentsDropDownList(user.DepartmentId)
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(UserProfileViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            // 仅验证客户端校验通过的情形
            if (!ModelState.IsValid)
            {
                model.DepartmentList = DepartmentsDropDownList(model.DepartmentId);
                return View(model);
            }

            bool anyUpdated = false;

            // 更新手机号（仅在变化时）
            if (user.PhoneNumber != model.PhoneNumber)
            {
                var phoneResult = await _userManager.SetPhoneNumberAsync(user, model.PhoneNumber);
                if (!phoneResult.Succeeded)
                {
                    foreach (var error in phoneResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
                else
                {
                    anyUpdated = true;
                }
            }

            // 更新邮箱（仅在变化时）
            if (user.Email != model.Email)
            {
                var emailResult = await _userManager.SetEmailAsync(user, model.Email);
                if (!emailResult.Succeeded)
                {
                    foreach (var error in emailResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
                else
                {
                    // 为安全起见，通常应要求邮箱确认流程；这里将 EmailConfirmed 置为 false 并保存。
                    user.EmailConfirmed = false;
                    var upd = await _userManager.UpdateAsync(user);
                    if (!upd.Succeeded)
                    {
                        foreach (var error in upd.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                    else
                    {
                        anyUpdated = true;
                    }
                }
            }

            if (!ModelState.IsValid)
            {
                model.DepartmentList = DepartmentsDropDownList(model.DepartmentId);
                return View(model);
            }

            if (anyUpdated)
            {
                await _signInManager.RefreshSignInAsync(user);
                ViewBag.Message = "更新成功";
            }
            else
            {
                ViewBag.Message = "未发现变更";
            }

            model.Id = user.Id;
            model.UserName = user.UserName;
            model.Name = user.Name;
            model.DepartmentId = user.DepartmentId;
            model.DepartmentList = DepartmentsDropDownList(user.DepartmentId);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

    }
}
