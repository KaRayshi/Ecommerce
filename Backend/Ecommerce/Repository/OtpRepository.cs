using Ecommerce.Data;
using Ecommerce.Interfaces;
using Ecommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Repository
{
    public class OtpRepository : IOtpRepository
    {
        private readonly ApplicationDbContext _context;

        public OtpRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task CreateOtpAsync(OtpVerification otp)
        {
            await _context.OtpVerifications.AddAsync(otp);
            await _context.SaveChangesAsync();
        }

        public async Task<OtpVerification?> GetLatestOtpByEmailAsync(string email)
        {
            return await _context.OtpVerifications
                .Where(o => o.Email == email)
                .OrderByDescending(o => o.CreatedAt)
                .FirstOrDefaultAsync();
        }

        public async Task UpdateOtpAsync(OtpVerification otp)
        {
            _context.OtpVerifications.Update(otp);
            await _context.SaveChangesAsync();
        }
    }
}
