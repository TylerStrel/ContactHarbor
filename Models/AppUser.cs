using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContactHarbor.Models;

public class AppUser : IdentityUser
{
    [Required]
    [Display(Name = "First Name")]
    [StringLength(50, ErrorMessage = "First Name cannot exceed 50 characters.")]
    [RegularExpression(@"^[a-zA-Z-' ]+$", ErrorMessage = "First Name can only contain alphabetical characters, hyphens, and apostrophes.")]
    public string? FirstName { get; set; }

    [Required]
    [Display(Name = "Last Name")]
    [StringLength(50, ErrorMessage = "Lirst Name cannot exceed 50 characters.")]
    [RegularExpression(@"^[a-zA-Z-' ]+$", ErrorMessage = "Lirst Name can only contain alphabetical characters, hyphens, and apostrophes.")]
    public string? LastName { get; set; }

    [NotMapped]
    [Display(Name = "Full Name")]
    public string? FullName { get { return $"{FirstName} {LastName}"; } }
}
