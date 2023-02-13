using System.ComponentModel.DataAnnotations;

namespace TasksHandler.Models
{
    public class RegisterDTO
    {
        [Required(ErrorMessage ="The field {0} is required")]
        [EmailAddress(ErrorMessage ="Email not valid")]
        public string Email { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
