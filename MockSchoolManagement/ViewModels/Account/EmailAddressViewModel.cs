using System.ComponentModel.DataAnnotations;

namespace MockSchoolManagement.ViewModels.Account
{
    public class EmailAddressViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
