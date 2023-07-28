using ContactHarbor.Models;
using ContactHarbor.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ContactHarbor.Controllers;

public class CategoriesController : Controller
{
    private readonly ICategoryService _categoryService;
    private readonly UserManager<AppUser> _userManager;

    public CategoriesController(ICategoryService categoryService, UserManager<AppUser> userManager)
    {
        _categoryService = categoryService;
        _userManager = userManager;
    }

    // GET: Categories
    public async Task<IActionResult> Index()
    {
        var categories = await _categoryService.GetAllCategoriesForUserAsync(_userManager.GetUserId(User)!);

        return View(categories);
    }

    // GET: Categories/Details/5
    public async Task<IActionResult> Details(Guid? id)
    {
        if (id is null) return NotFound();

        var category = await _categoryService.GetCategoryByIdAsync(id.Value);

        if (category is null) return NotFound();

        return View(category);
    }

    // GET: Categories/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Categories/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,AppUserId,Name")] Category category)
    {
        ModelState.Remove("AppUserId");

        if (ModelState.IsValid)
        {
            var results = await _categoryService.CreateCategoryAsync(category, _userManager.GetUserId(User)!);

            if (results)
            {
                return RedirectToAction(nameof(Index));
            }
        }

        return View(category);
    }

    // GET: Categories/Edit/5
    public async Task<IActionResult> Edit(Guid? id)
    {
        if (id is null) return NotFound();

        var category = await _categoryService.GetCategoryByIdAsync(id.Value);

        if (category is null) return NotFound();

        return View(category);
    }

    // POST: Categories/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, [Bind("Id,AppUserId,Name")] Category category)
    {
        if (id != category.Id) return NotFound();

        ModelState.Remove("AppUserId");

        if (ModelState.IsValid)
        {
            var results = await _categoryService.UpdateCategoryAsync(category);

            if (results)
            {
                return RedirectToAction(nameof(Index));
            }
        }

        return View(category);
    }

    // GET: Categories/Delete/5
    public async Task<IActionResult> Delete(Guid? id)
    {
        if (id is null) return NotFound();

        var category = await _categoryService.GetCategoryByIdAsync(id.Value);

        if (category is null) return NotFound();

        return View(category);
    }

    // POST: Categories/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        var results = await _categoryService.DeleteCategoryAsync(id);

        if (results)
        {
            return RedirectToAction(nameof(Index));
        }

        return View(await _categoryService.GetCategoryByIdAsync(id));
    }
}
