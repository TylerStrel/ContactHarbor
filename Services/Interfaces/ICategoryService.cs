using ContactHarbor.Models;

namespace ContactHarbor.Services.Interfaces;

public interface ICategoryService
{
    Task<bool> CreateCategoryAsync(Category category, string userId);
    Task<List<Category>> GetAllCategoriesForUserAsync(string userId);
    Task<Category> GetCategoryByIdAsync(Guid categoryId);
    Task<bool> UpdateCategoryAsync(Category category);
    Task<bool> DeleteCategoryAsync(Guid categoryId);
    Task<bool> AddContactToCategoriesAsync(Contact contact, List<Guid> categoryIds);
    Task<bool> RemoveAllCategoriesFromContactAsync(Contact contact);
}
