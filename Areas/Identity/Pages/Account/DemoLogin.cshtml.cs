using ContactHarbor.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ContactHarbor.Areas.Identity.Pages.Account
{
    public class DemoLogin : PageModel
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IConfiguration _configuration;

        public DemoLogin(SignInManager<AppUser> signInManager, IConfiguration configuration)
        {
            _signInManager = signInManager;
            _configuration = configuration;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            if (_signInManager.IsSignedIn(User))
            {
                return LocalRedirect("~/Index");
            }

            var result = await _signInManager.PasswordSignInAsync("demo@contactharbor.com", _configuration["DemoUserPassword"]!, isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded) 
            {
                return RedirectToAction("Index", "Contacts");

            }

            TempData["ErrorMessage"] = "Demo login failed.";
            return RedirectToPage("Index");
        }
    }
}
