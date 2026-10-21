using CYHBrandRepo;
using CYHBrandUI.Helpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;

namespace CYHBrandUI.Controllers
{
    public class BaseController : Controller
    {
        protected readonly IHttpContextAccessor _http;
        protected readonly IConfiguration       _config;
        protected readonly IUnitOfWork          UOF;
        protected readonly IUrlEncryptor        Enc;
        protected ISession Session => _http.HttpContext!.Session;

        public BaseController(IHttpContextAccessor http, IConfiguration config,
                              IUnitOfWork uof, IUrlEncryptor enc)
        {
            _http   = http;
            _config = config;
            UOF     = uof;
            Enc     = enc;
        }

        // ── Current logged-in user ─────────────────────────────────────────
        protected CustomPrincipal? CurrentUser
        {
            get
            {
                var ud = User?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.UserData)?.Value;
                return ud is null ? null : JsonConvert.DeserializeObject<CustomPrincipal>(ud);
            }
        }

        // ── Sign in ────────────────────────────────────────────────────────
        protected async Task SignInUser(CustomPrincipal user, bool isPersistent = false)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.UserData, JsonConvert.SerializeObject(user)),
                new(ClaimTypes.Email,    user.Email  ?? ""),
                new(ClaimTypes.Name,     user.Name   ?? ""),
                new(ClaimTypes.Role,     user.Roles?.FirstOrDefault() ?? "Doctor"),
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity),
                new AuthenticationProperties
                {
                    IsPersistent = isPersistent,
                    AllowRefresh = true,
                    ExpiresUtc   = DateTimeOffset.UtcNow.AddHours(8)
                });
        }

        // ── Sign out ───────────────────────────────────────────────────────
        protected async Task SignOutUser() =>
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        // ── JSON helpers ───────────────────────────────────────────────────
        protected IActionResult JsonOk(string msg = "Success", object? data = null) =>
            Json(new { isSuccess = true,  msg, data });

        protected IActionResult JsonFail(string? msg = "Error occurred") =>
            Json(new { isSuccess = false, msg });
    }
}
