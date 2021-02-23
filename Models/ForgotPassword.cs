using System.ComponentModel.DataAnnotations;

namespace Model
{
    public class ForgotPassword
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
