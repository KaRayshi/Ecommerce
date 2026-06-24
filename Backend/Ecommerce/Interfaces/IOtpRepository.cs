using Ecommerce.Models;

namespace Ecommerce.Interfaces
{
    public interface IOtpRepository
    {
        Task CreateOtpAsync(OtpVerification otp);
        Task<OtpVerification?> GetLatestOtpByEmailAsync(string email);
        Task UpdateOtpAsync(OtpVerification otp);
    }
}
