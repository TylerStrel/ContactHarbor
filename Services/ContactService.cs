using ContactHarbor.Data;
using ContactHarbor.Models;
using ContactHarbor.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ContactHarbor.Services;

public class ContactService : IContactService
{
    private readonly ApplicationDbContext _context;
    private readonly IImageService _imageService;

    public ContactService(ApplicationDbContext context, IImageService imageService)
    {
        _context = context;
        _imageService = imageService;
    }

    public async Task<bool> CreateContactAsync(Contact contact, string userId)
    {
        try
        {
            if (userId is null) throw new ArgumentNullException(nameof(userId), "User ID cannot be null");

            contact.AppUserId = userId;
            contact.DateOfBirth = contact.DateOfBirth.ToUniversalTime();
            contact.Created = DateTime.UtcNow;

            if (contact.Image is not null)
            {
                contact.ImageData = await _imageService.ConvertFileToByteArrayAsync(contact.Image);
                contact.ImageName = contact.Image.Name;
                contact.ImageType = contact.Image.ContentType;
            }
            else
            {
                await SetDefaultContactImage(contact);
            }

            await _context.Contacts.AddAsync(contact);
            var results = await _context.SaveChangesAsync();
            return results > 0;
        }
        catch
        {
            throw;
        }
    }

    public async Task<List<Contact>> GetAllContactsAsync(string userId)
    {
        try
        {
            if (userId is null) throw new ArgumentNullException(nameof(userId), "User ID cannot be null");

            var contacts = await _context.Contacts.Where(c => c.AppUserId == userId)
                .Include(c => c.Categories)
                .ToListAsync();

            return contacts;
        }
        catch
        {
            throw;
        }
    }

    public async Task<Contact> GetContactByIdAsync(Guid contactId)
    {
        try
        {
            var contact = await _context.Contacts
                .Where(c => c.Id == contactId)
                .Include(c => c.Categories)
                .SingleOrDefaultAsync();

            if (contact is null) throw new KeyNotFoundException("Contact not found");

            return contact;
        }
        catch
        {
            throw;
        }
    }

    public async Task<bool> UpdateContactAsync(Contact contact, bool clearImage)
    {
        try
        {
            if (clearImage)
            {
                await SetDefaultContactImage(contact);
            }
            else if (contact.Image is not null)
            {
                contact.ImageData = await _imageService.ConvertFileToByteArrayAsync(contact.Image);
                contact.ImageName = contact.Image.Name;
                contact.ImageType = contact.Image.ContentType;
            }
            else if (contact.ImageData is not null && contact.ImageName is not null && contact.ImageType is not null)
            {
                contact.ImageData = contact.ImageData;
                contact.ImageName = contact.ImageName;
                contact.ImageType = contact.ImageType;
            }

            contact.DateOfBirth = contact.DateOfBirth.UtcDateTime;
            contact.Created = contact.Created.UtcDateTime;

            _context.Contacts.Update(contact!);
            var results = await _context.SaveChangesAsync();
            return results > 0;
        }
        catch
        {
            throw;
        }
    }

    public async Task<bool> DeleteContactAsync(Guid contactId)
    {
        try
        {
            var contact = await GetContactByIdAsync(contactId);

            _context.Contacts.Remove(contact);
            var results = await _context.SaveChangesAsync();
            return results > 0;
        }
        catch
        {
            throw;
        }
    }

    public async Task<IEnumerable<Contact>> SearchContactsAsync(string searchString, string userId)
    {
        try
        {
            searchString = searchString.ToLower();
            var contacts = await _context.Contacts
                .Where(c => c.AppUserId == userId &&
                            (c.FirstName!.ToLower().Contains(searchString) ||
                             c.LastName!.ToLower().Contains(searchString) ||
                             c.Email!.ToLower().Contains(searchString) ||
                             c.Address1!.ToLower().Contains(searchString) ||
                             c.Address2!.ToLower().Contains(searchString) ||
                             c.City!.ToLower().Contains(searchString) ||
                             c.State!.ToLower().Contains(searchString) ||
                             c.ZipCode!.ToLower().Contains(searchString) ||
                             c.Categories.Any(cat => cat.Name!.ToLower().Contains(searchString)) ||
                             (c.PhoneNumber != null && c.PhoneNumber.ToLower().Contains(searchString))))
                .Include(c => c.Categories)
                .ToListAsync();

            return contacts;
        }
        catch
        {
            throw;
        }
    }

    public async Task<IEnumerable<Contact>> GetAllContactsInCategoryAsync(Guid categoryId, string userId)
    {
        try
        {
            if (userId is null) throw new ArgumentNullException(nameof(userId), "User ID cannot be null");

            var contacts = await _context.Contacts.Where(c => c.AppUserId == userId)
                .Include(c => c.Categories)
                .Where(c => c.Categories.Any(cat => cat.Id == categoryId))
                .ToListAsync();

            return contacts;
        }
        catch
        {
            throw;
        }
    }


    private async Task SetDefaultContactImage(Contact contact)
    {
        string pathToDefaultImage = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "DefaultContactImage.png");
        contact.ImageData = await File.ReadAllBytesAsync(pathToDefaultImage);
        contact.ImageName = "DefaultContactImage.png";
        contact.ImageType = "image/png";
    }
}
