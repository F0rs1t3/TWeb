using System.ComponentModel.DataAnnotations;

using System.ComponentModel.DataAnnotations;

namespace TWeb.Models.ViewModels
{
    public class EditProfileViewModel
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string UserName { get; set; }

        [Required(ErrorMessage = "Password is required to confirm changes.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
