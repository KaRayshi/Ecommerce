using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Dto.User
{
    public class RequestOtpDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}