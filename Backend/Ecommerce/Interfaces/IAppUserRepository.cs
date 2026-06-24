using Ecommerce.Dto.User;
using Ecommerce.Helpers;
using Ecommerce.Models;

namespace Ecommerce.Interfaces
{
    public interface IAppUserRepository
    {
        Task<List<AppUser>> GetAllUserAsync(AppUserQueryObject query);
        Task<AppUser> GetUserByIdAsync(string id);
        Task<AppUser> UpdateUserAsync(string userID, UpdateUserDto updateUser);
        Task<AppUser> deleteUserAsync(string userID);
    }
}
