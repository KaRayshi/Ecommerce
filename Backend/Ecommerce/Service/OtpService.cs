using System.Security.Cryptography;

namespace Ecommerce.Service
{
    public class OtpService
    {
        public string GenerateSecureOtp()
        {
            int otp = RandomNumberGenerator.GetInt32(100000, 1000000);
            return otp.ToString();
        }
    }
}
