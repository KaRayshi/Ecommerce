using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Dtos.Account
{
    public class VerifyEmailDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string OtpCode { get; set; }
    }
}