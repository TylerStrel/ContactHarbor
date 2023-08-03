using ContactHarbor.Models;
using ContactHarbor.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ContactHarbor.Controllers;

[Authorize]
public class ContactsController : Controller
{
    private readonly IContactService _contactService;
    private readonly ICategoryService _categoryService;
    private readonly IEmailSender _emailService;
    private readonly UserManager<AppUser> _userManager;

    public ContactsController(IContactService contactService, ICategoryService categoryService, IEmailSender emailService, UserManager<AppUser> userManager)
    {
        _contactService = contactService;
        _categoryService = categoryService;
        _emailService = emailService;
        _userManager = userManager;
    }

    // GET: Contacts
    public async Task<IActionResult> Index()
    {
        var contacts = await _contactService.GetAllContactsAsync(_userManager.GetUserId(User)!);

        var categories = await _categoryService.GetAllCategoriesForUserAsync(_userManager.GetUserId(User)!);
        ViewData["CategoryList"] = new SelectList(categories, "Id", "Name");


        return View(contacts);
    }

    // GET: Contacts/Create
    public async Task<IActionResult> Create()
    {
        await CreateViewDataSelectLists();
        return View();
    }

    // POST: Contacts/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,AppUserId,FirstName,LastName,DateOfBirth,Address1,Address2,City,State,ZipCode,Email,PhoneNumber,Image,ImageName,ImageData,ImageType,Created")] Contact contact, List<Guid> categories)
    {
        ModelState.Remove("AppUserId");

        if (ModelState.IsValid)
        {
            var results = await _contactService.CreateContactAsync(contact, _userManager.GetUserId(User)!);

            if (results)
            {
                if (categories.Count > 0)
                {
                    await _categoryService.AddContactToCategoriesAsync(contact, categories);
                }

                TempData["SuccessMessage"] = "Contact created successfully.";
                return RedirectToAction(nameof(Index));
            }
        }

        TempData["ErrorMessage"] = "Unable to create contact, please try again.";
        await CreateViewDataSelectLists();
        return View(contact);
    }

    // GET: Contacts/Edit/5
    public async Task<IActionResult> Edit(Guid? id)
    {
        if (id is null) return NotFound();

        var contact = await _contactService.GetContactByIdAsync(id.Value);

        if (contact is null) return NotFound();

        await CreateViewDataSelectLists(contact);
        return View(contact);
    }

    // POST: Contacts/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, [Bind("Id,AppUserId,FirstName,LastName,DateOfBirth,Address1,Address2,City,State,ZipCode,Email,PhoneNumber,Image,ImageName,ImageData,ImageType,Created")] Contact contact, List<Guid> categories, bool clearImageFlag)
    {
        if (id != contact.Id) return NotFound();

        ModelState.Remove("AppUserId");

        if (ModelState.IsValid)
        {
            var results = await _contactService.UpdateContactAsync(contact, clearImageFlag);

            if (results)
            {
                if (categories.Count == 0)
                {
                    await _categoryService.RemoveAllCategoriesFromContactAsync(contact);
                }
                else
                {
                    await _categoryService.RemoveAllCategoriesFromContactAsync(contact);
                    await _categoryService.AddContactToCategoriesAsync(contact, categories);
                }

                TempData["SuccessMessage"] = "Contact updated successfully.";
                return RedirectToAction(nameof(Index));
            }
        }

        TempData["ErrorMessage"] = "Unable to update contact, please try again.";
        await CreateViewDataSelectLists(contact);
        return View(contact);
    }

    // GET: Contacts/Delete/5
    public async Task<IActionResult> Delete(Guid? id)
    {
        if (id is null) return NotFound();

        var contact = await _contactService.GetContactByIdAsync(id.Value);

        if (contact == null) return NotFound();

        return View(contact);
    }

    // POST: Contacts/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        var results = await _contactService.DeleteContactAsync(id);

        if (results)
        {
            TempData["SuccessMessage"] = "Contact deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        TempData["ErrorMessage"] = "Unable to delete contact, please try again.";
        return View(await _contactService.GetContactByIdAsync(id));
    }

    [HttpGet]
    public async Task<IActionResult> Email(Guid id)
    {
        var contact = await _contactService.GetContactByIdAsync(id);

        if (contact is null) return NotFound();

        var viewModel = new EmailContactViewModel
        {
            Contact = contact
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Email(Guid id, EmailContactViewModel model)
    {
        await _emailService.SendEmailAsync(model.Contact!.Email!, model.Subject!, model.Message!);

        TempData["SuccessMessage"] = "Email sent successfully.";
        return RedirectToAction(nameof(Index));
    }


    [HttpGet]
    public async Task<IActionResult> Search(string searchString)
    {
        var resluts = await _contactService.SearchContactsAsync(searchString, _userManager.GetUserId(User)!);

        if (!resluts.Any())
        {
            resluts = new List<Contact>();
        }

        return PartialView("_ContactTable", resluts);
    }

    public async Task<IActionResult> FilterByCategory(Guid categoryId)
    {
        var resluts = await _contactService.GetAllContactsInCategoryAsync(categoryId, _userManager.GetUserId(User)!);
        return PartialView("_ContactTable", resluts);
    }

    private async Task CreateViewDataSelectLists(Contact? contact = null)
    {
        var categories = await _categoryService.GetAllCategoriesForUserAsync(_userManager.GetUserId(User)!);

        if (contact is null)
        {
            ViewData["Categories"] = new MultiSelectList(categories, "Id", "Name");
        }
        else
        {
            ViewData["Categories"] = new MultiSelectList(categories, "Id", "Name", contact.Categories.Select(c => c.Id));
        }
    }
}
