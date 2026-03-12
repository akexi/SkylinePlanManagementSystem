using System.ComponentModel.DataAnnotations;

namespace MockSchoolManagement.ViewModels
{
    public class EmailAddressViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
