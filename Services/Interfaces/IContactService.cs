using ContactHarbor.Models;

namespace ContactHarbor.Services.Interfaces;

public interface IContactService
{
    Task<bool> CreateContactAsync(Contact contact, string userId);
    Task<List<Contact>> GetAllContactsAsync(string userId);
    Task<Contact> GetContactByIdAsync(Guid contactId);
    Task<bool> UpdateContactAsync(Contact contact);
    Task<bool> DeleteContactAsync(Guid contactId);
}
