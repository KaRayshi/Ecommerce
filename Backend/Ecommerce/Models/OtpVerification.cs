using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Models
{
    public class OtpVerification
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(6)]
        public string OtpCode { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime ExpiresAt { get; set; }

        // This is a great bonus flag! We can switch this to true once they 
        // successfully use the code so they can't use it twice.
        public bool IsUsed { get; set; } = false;
    }
}
