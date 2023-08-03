// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using ContactHarbor.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ContactHarbor.Areas.Identity.Pages.Account;

/// <summary>
///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
///     directly from your code. This API may change or be removed in future releases.
/// </summary>
[AllowAnonymous]
public class ResetPasswordConfirmationModel : PageModel
{
    private readonly SignInManager<AppUser> _signInManager;

    public ResetPasswordConfirmationModel(SignInManager<AppUser> signInManager)
    {
        _signInManager = signInManager;
    }

    /// <summary>
    ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public IActionResult OnGet()
    {
        if (_signInManager.IsSignedIn(User))
        {
            return LocalRedirect("~/Index");
        }

        return Page();
    }
}
