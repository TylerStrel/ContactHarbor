using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContactHarbor.Models;

public class Contact
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [Display(Name = "Owner User Id")]
    public string? AppUserId { get; set; }

    [Required]
    [Display(Name = "First Name")]
    [StringLength(50, ErrorMessage = "First Name cannot exceed 50 characters.")]
    [RegularExpression(@"^[a-zA-Z-' ]+$", ErrorMessage = "First Name can only contain alphabetical characters, hyphens, and apostrophes.")]
    public string? FirstName { get; set; }

    [Required]
    [Display(Name = "Last Name")]
    [StringLength(50, ErrorMessage = "Last Name cannot exceed 50 characters.")]
    [RegularExpression(@"^[a-zA-Z-' ]+$", ErrorMessage = "Last Name can only contain alphabetical characters, hyphens, and apostrophes.")]
    public string? LastName { get; set; }

    [NotMapped]
    [Display(Name = "Full Name")]
    public string? FullName { get { return $"{FirstName} {LastName}"; } }

    [Required]
    [Display(Name = "Birthday")]
    [DataType(DataType.Date)]
    public DateTimeOffset DateOfBirth { get; set; }

    [Display(Name = "Address 1")]
    [StringLength(100, ErrorMessage = "Address 1 cannot exceed 100 characters.")]
    public string? Address1 { get; set; }

    [Display(Name = "Address 2")]
    [StringLength(100, ErrorMessage = "Address 2 cannot exceed 100 characters.")]
    public string? Address2 { get; set; }

    [Display(Name = "City")]
    [StringLength(50, ErrorMessage = "City cannot exceed 50 characters.")]
    public string? City { get; set; }

    [Display(Name = "State")]
    [StringLength(50, ErrorMessage = "State cannot exceed 50 characters.")]
    public string? State { get; set; }

    [Display(Name = "Postal Code")]
    [RegularExpression(@"^\d{5}$", ErrorMessage = "Postal Code must be exactly 5 digits.")]
    public string? ZipCode { get; set; }

    [Required]
    [Display(Name = "Email Address")]
    [EmailAddress(ErrorMessage = "Invalid Email Address")]
    public string? Email { get; set; }

    [Display(Name = "Phone Number")]
    [Phone(ErrorMessage = "Invalid Phone Number")]
    public string? PhoneNumber { get; set; }

    [NotMapped]
    public IFormFile? Image { get; set; }

    [Display(Name = "Image Name")]
    public string? ImageName { get; set; }

    [Display(Name = "Image Data")]
    public byte[]? ImageData { get; set; }

    [Display(Name = "Image Type")]
    public string? ImageType { get; set; }

    [Required]
    public DateTimeOffset Created { get; set; }

    // Navigation Properties
    public virtual AppUser? AppUser { get; set; }
    public virtual ICollection<Category> Categories { get; set; } = new HashSet<Category>();
}
