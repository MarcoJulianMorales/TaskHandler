using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TasksHandler.Models;
using TasksHandler.Services;

namespace TasksHandler.Controllers
{
    public class UsersController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly ApplicationDbContext applicationDbContext;

        public UsersController(UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ApplicationDbContext applicationDbContext)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.applicationDbContext = applicationDbContext;
        }

        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register(RegisterDTO register)
        {
            if (!ModelState.IsValid)
            {
                return View(register);
            }

            var user = new IdentityUser() { Email = register.Email, UserName = register.Email };
            var result = await userManager.CreateAsync(user, password: register.Password);

            if (result.Succeeded)
            {
                await signInManager.SignInAsync(user, isPersistent: true);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(register);
            }
        }

        [AllowAnonymous]
        public IActionResult Login(string message = null)
        {
            if(message is not null)
            {
                ViewData["message"] = message;
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginDTO login)
        {
            if (!ModelState.IsValid)
            {
                return View(login);
            }

            var result = await signInManager.PasswordSignInAsync(login.Email, login.Password, login.Rememberme, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "User name or Password is incorrect.");
                    return View(login);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        [HttpGet]
        public ChallengeResult ExternalLogin(string provider, string returnUrl = null)
        {
            var UrlDirection = Url.Action("RegisterExternalUser", values: new { returnUrl });
            var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, returnUrl);
            return new ChallengeResult(provider, properties);
        }

        [AllowAnonymous]
        public async Task<IActionResult> RegisterExternalUser(string returnUrl = null, string remoteError = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            var message = "";

            if(remoteError is not null)
            {
                message = $"External provider error: {remoteError}";
                return RedirectToAction("Login", routeValues: new { message });
            }

            var info = await signInManager.GetExternalLoginInfoAsync();
            if(info is null)
            {
                message = "Error loading login external data";
                return RedirectToAction("Login", routeValues: new { message });
            }
            var ExternalLoginResult = await signInManager.ExternalLoginSignInAsync(info.LoginProvider,
                info.ProviderKey, isPersistent: true, bypassTwoFactor: true);

            if(ExternalLoginResult.Succeeded)
            {
                return LocalRedirect(returnUrl);
            }

            string email = "";

            if(info.Principal.HasClaim(c => c.Type == ClaimTypes.Email))
            {
                email = info.Principal.FindFirstValue(ClaimTypes.Email);
            }
            else
            {
                message = "Error reading the provider user email.";
                return RedirectToAction("Login", routeValues: new { message });
            }

            var user = new IdentityUser { Email = email, UserName = email };
            var CreatingUserResult = await userManager.CreateAsync(user);

            if(!CreatingUserResult.Succeeded)
            {
                message = CreatingUserResult.Errors.First().Description;
                return RedirectToAction("Login", routeValues: new { message });
            }

            var AddLoginResult = await userManager.AddLoginAsync(user, info);

            if(AddLoginResult.Succeeded)
            {
                await signInManager.SignInAsync(user, isPersistent: true, info.LoginProvider);
                return LocalRedirect(returnUrl);
            }

            message = "An error has occurred adding the login";
                return RedirectToAction("Login", routeValues: new { message });
        }

        [HttpGet]
        [Authorize(Roles =Constants.RoleAdmin)]
        public async Task<IActionResult> List(string message = null)
        {
            var users = await applicationDbContext.Users.Select(u => new UserDTO
            {
                Email = u.Email
            }).ToListAsync();

            var model = new UsersListDTO();
            model.Users = users;
            model.Message= message;
            return View(model);
        }
        [HttpPost]
        [Authorize(Roles = Constants.RoleAdmin)]
        public async Task<IActionResult> MakeAdmin(string email)
        {
            var user = await applicationDbContext.Users.Where(u => u.Email == email).FirstOrDefaultAsync();
            if (user == null)
            {
                return NotFound();
            }
            await userManager.AddToRoleAsync(user, Constants.RoleAdmin);

            return RedirectToAction("List", routeValues: new { message = "Role successfully assigned to " + email });
        }

        [HttpPost]
        [Authorize(Roles = Constants.RoleAdmin)]
        public async Task<IActionResult> RemoveAdmin(string email)
        {
            var user = await applicationDbContext.Users.Where(u => u.Email == email).FirstOrDefaultAsync();
            if (user == null)
            {
                return NotFound();
            }
            await userManager.RemoveFromRoleAsync(user, Constants.RoleAdmin);

            return RedirectToAction("List", routeValues: new { message = "Role successfully removed from " + email });
        }
    }
}
