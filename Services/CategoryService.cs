using ContactHarbor.Data;
using ContactHarbor.Models;
using ContactHarbor.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ContactHarbor.Services;

public class CategoryService : ICategoryService
{
    private readonly ApplicationDbContext _context;
    private readonly IContactService _contactService;

    public CategoryService(ApplicationDbContext context, IContactService contactService)
    {
        _context = context;
        _contactService = contactService;

    }

    public async Task<bool> CreateCategoryAsync(Category category, string userId)
    {
        try
        {
            if (userId is null) throw new ArgumentNullException(nameof(userId), "User ID cannot be null");

            category.AppUserId = userId;
            await _context.Categories.AddAsync(category);
            var results = await _context.SaveChangesAsync();
            return results > 0;
        }
        catch
        {
            throw;
        }
    }

    public async Task<Category> GetCategoryByIdAsync(Guid categoryId)
    {
        try
        {
            var category = await _context.Categories
                .Where(c => c.Id == categoryId)
                .SingleOrDefaultAsync();

            if (category is null) throw new KeyNotFoundException("Category not found");

            return category;
        }
        catch
        {
            throw;
        }
    }

    public async Task<List<Category>> GetAllCategoriesForUserAsync(string userId)
    {
        try
        {
            if (userId is null) throw new ArgumentNullException(nameof(userId), "User ID cannot be null");

            var categories = await _context.Categories
                .Where(c => c.AppUserId == userId)
                .ToListAsync();

            return categories;
        }
        catch
        {
            throw;
        }
    }

    public async Task<bool> UpdateCategoryAsync(Category category)
    {
        try
        {
            _context.Categories.Update(category);
            var results = await _context.SaveChangesAsync();
            return results > 0;
        }
        catch
        {
            throw;
        }
    }

    public async Task<bool> DeleteCategoryAsync(Guid categoryId)
    {
        try
        {
            var category = await _context.Categories.FindAsync(categoryId);

            if (category is null) throw new KeyNotFoundException("Category not found");

            _context.Categories.Remove(category);
            var results = await _context.SaveChangesAsync();
            return results > 0;
        }
        catch
        {
            throw;
        }
    }

    public async Task<bool> AddContactToCategoriesAsync(Contact contact, List<Guid> categoryIds)
    {
        try
        {
            foreach (var categoryId in categoryIds)
            {
                var category = await GetCategoryByIdAsync(categoryId);
                category.Contacts.Add(contact);
            }

            var results = await _context.SaveChangesAsync();
            return results > 0;
        }
        catch
        {
            throw;
        }
    }

    public async Task<bool> RemoveAllCategoriesFromContactAsync(Contact contact)
    {
        try
        {
            var _contact = await _contactService.GetContactByIdAsync(contact.Id);

            _contact.Categories.Clear();
            _context.Update(_contact);
            var results = await _context.SaveChangesAsync();
            return results > 0;
        }
        catch
        {
            throw;
        }
    }
}
