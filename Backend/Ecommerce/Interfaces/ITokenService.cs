using Ecommerce.Models;

namespace Ecommerce.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(AppUser appUser, IList<string> roles);
    }
}
