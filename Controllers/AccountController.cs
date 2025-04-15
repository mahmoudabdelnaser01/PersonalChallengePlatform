using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PersonalChallengePlatform.Models;
using PersonalChallengePlatform.Models.ViewModels;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace PersonalChallengePlatform.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Dashboard");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = new ApplicationUser 
                    { 
                        UserName = model.Email, 
                        Email = model.Email,
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        CreatedAt = DateTime.UtcNow
                    };
                    
                    var result = await _userManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {
                        _logger.LogInformation("User {Email} created a new account.", model.Email);
                        
                        // Add user to default role if needed
                        await _userManager.AddToRoleAsync(user, "User");
                        
                        // Add FirstName claim
                        await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("FirstName", user.FirstName));
                        
                        await _signInManager.SignInAsync(user, isPersistent: true);
                        _logger.LogInformation("User {Email} logged in.", model.Email);
                        
                        return RedirectToAction("Index", "Dashboard");
                    }
                    
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                        _logger.LogWarning("User registration failed for {Email}: {Error}", model.Email, error.Description);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred during user registration for {Email}", model.Email);
                    ModelState.AddModelError(string.Empty, "An error occurred during registration. Please try again.");
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Dashboard");
            }
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                try
                {
                    var result = await _signInManager.PasswordSignInAsync(
                        model.Email, 
                        model.Password, 
                        model.RememberMe, 
                        lockoutOnFailure: true);
                        
                    if (result.Succeeded)
                    {
                        _logger.LogInformation("User {Email} logged in.", model.Email);
                        
                        // Add FirstName claim if not present
                        var user = await _userManager.FindByEmailAsync(model.Email);
                        var claims = await _userManager.GetClaimsAsync(user);
                        if (!claims.Any(c => c.Type == "FirstName"))
                        {
                            await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("FirstName", user.FirstName));
                        }
                        
                        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                        {
                            return Redirect(returnUrl);
                        }
                        return RedirectToAction("Index", "Dashboard");
                    }
                    if (result.IsLockedOut)
                    {
                        _logger.LogWarning("User {Email} account locked out.", model.Email);
                        return RedirectToAction(nameof(Lockout));
                    }
                    
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    _logger.LogWarning("Invalid login attempt for user {Email}", model.Email);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred during login for {Email}", model.Email);
                    ModelState.AddModelError(string.Empty, "An error occurred during login. Please try again.");
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Lockout()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                _logger.LogInformation("User {Email} logged out.", user.Email);
            }
            
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}