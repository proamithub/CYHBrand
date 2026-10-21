using CYHBrandModel.DTO;
using CYHBrandRepo;
using CYHBrandUI.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CYHBrandUI.Controllers
{
    public class AccountController : BaseController
    {
        public AccountController(IHttpContextAccessor http, IConfiguration config,
                                 IUnitOfWork uof, IUrlEncryptor enc)
            : base(http, config, uof, enc) { }

        [AllowAnonymous][HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User?.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Home");
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [AllowAnonymous][HttpPost][ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDTO model, string? returnUrl = null)
        {
            if (!ModelState.IsValid) return View(model);
            var doctor = await UOF.DoctorRepo.AuthenticateAsync(model.Email!, model.Password!);
            if (doctor == null)
            { ModelState.AddModelError("", "Invalid email or password."); return View(model); }
            if (doctor.IsActive == false)
            { ModelState.AddModelError("", "Your account is inactive. Please contact admin."); return View(model); }

            await SignInUser(new CustomPrincipal
            {
                UserId            = doctor.DoctorId,
                Name              = doctor.DisplayName ?? doctor.FullName,
                Email             = doctor.Email,
                MobileNo          = doctor.MobileNo,
                Roles             = new[] { "Doctor" },
                ProfilePic        = doctor.ProfilePic,
                Specialty         = doctor.Specialty,
                ClinicName        = doctor.ClinicName,
                IsProfileComplete = doctor.IsProfileComplete
            }, model.RememberMe);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous][HttpGet]
        public IActionResult Register()
        {
            if (User?.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Home");
            return View(new DoctorMasterDTO());
        }

        [AllowAnonymous][HttpPost][ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(DoctorMasterDTO model)
        {
            if (!ModelState.IsValid) return View(model);
            var exists = await UOF.DoctorRepo.EmailExistsAsync(model.Email!);
            if (exists)
            { ModelState.AddModelError("Email", "Email already registered."); return View(model); }
            var result = await UOF.DoctorRepo.RegisterAsync(model);
            if (result.IsSuccess)
            { TempData["SuccessMsg"] = "Registration successful! Please login."; return RedirectToAction("Login"); }
            ModelState.AddModelError("", result.Message);
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await SignOutUser();
            return RedirectToAction("Login");
        }

        [AllowAnonymous]
        public IActionResult AccessDenied() => View();
    }
}
