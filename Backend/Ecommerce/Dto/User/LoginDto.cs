using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Dto.User
{
    public class LoginDto
    {
        [Required]
        public string UsernameOrEmail { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;


    }
}
