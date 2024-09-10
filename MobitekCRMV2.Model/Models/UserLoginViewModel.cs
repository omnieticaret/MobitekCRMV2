using System.ComponentModel.DataAnnotations;

namespace MobitekCRMV2.Model.Models
{
    public class UserLoginViewModel
    {
        [Required]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public string ErrorMessage { get; set; }

    }
}
