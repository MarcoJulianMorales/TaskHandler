using System.ComponentModel.DataAnnotations;

namespace TasksHandler.Models
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "Field {0} is required")]
        [EmailAddress(ErrorMessage ="Email not valid")]
        public string Email { get; set; }

        [Required (ErrorMessage ="Field {0} is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        
        [Display(Name ="Remember me")]
        public bool Rememberme { get; set; }
    }
}
