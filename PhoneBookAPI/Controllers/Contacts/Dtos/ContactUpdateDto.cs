using System.ComponentModel.DataAnnotations;

namespace PhoneBookAPI.Controllers.Contacts.Dtos
{
    public class ContactUpdateDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [MaxLength(20)]
        [RegularExpression(@"^\+?[1-9]\d{1,14}$", ErrorMessage = "Invalid phone number format")]
        public string PhoneNumber { get; set; }

        [EmailAddress]
        public string? Email { get; set; }
    }
}
