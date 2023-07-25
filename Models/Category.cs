using System.ComponentModel.DataAnnotations;

namespace ContactHarbor.Models;

public class Category
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [Display(Name = "Owner User Id")]
    public string? AppUserId { get; set; }

    [Required]
    [Display(Name = "Category Name")]
    [StringLength(50, ErrorMessage = "Category Name cannot exceed 50 characters.")]
    public string? Name { get; set; }

    // Navigation Properties
    public virtual AppUser? AppUser { get; set; }
    public virtual ICollection<Contact> Contacts { get; set; } = new HashSet<Contact>();
}
