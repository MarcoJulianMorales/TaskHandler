using System.ComponentModel.DataAnnotations;

namespace TasksHandler.Models
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "Error.Required")]
        [EmailAddress(ErrorMessage ="Error.Email")]
        public string Email { get; set; }

        [Required (ErrorMessage = "Error.Required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        
        [Display(Name ="Remember me")]
        public bool Rememberme { get; set; }
    }
}
