using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Policy;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace BeyonceConcert.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // ===================== LOGIN =====================

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string email, string password, string returnUrl = null)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                TempData["Error"] = "Email and password are required.";
                ViewData["ReturnUrl"] = returnUrl;
                return View();
            }

            // 🔥 Find user by email first
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                TempData["Error"] = "Invalid login attempt.";
                ViewData["ReturnUrl"] = returnUrl;
                return View();
            }

            var result = await _signInManager.PasswordSignInAsync(
                user.UserName,
                password,
                isPersistent: false,
                lockoutOnFailure: false);

            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction("SelectSeat", "Tickets");
            }

            TempData["Error"] = "Invalid login attempt.";
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // ===================== REGISTER =====================

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Register(string email, string password, string returnUrl = null)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                TempData["Error"] = "Email and password are required.";
                ViewData["ReturnUrl"] = returnUrl;
                return View();
            }

            var user = new IdentityUser
            {
                UserName = email,
                Email = email
            };

            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                TempData["Success"] = "Registration successful. Please log in.";

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return RedirectToAction("Login", new { returnUrl });

                return RedirectToAction("Login");
            }

            TempData["Error"] = string.Join(" ", result.Errors.Select(e => e.Description));
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // ===================== LOGOUT =====================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}