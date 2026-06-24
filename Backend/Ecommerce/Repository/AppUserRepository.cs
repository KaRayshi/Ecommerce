using Ecommerce.Data;
using Ecommerce.Dto.User;
using Ecommerce.Helpers;
using Ecommerce.Interfaces;
using Ecommerce.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Ecommerce.Mappers;

namespace Ecommerce.Repository
{
    public class AppUserRepository : IAppUserRepository
    {
        private readonly UserManager<AppUser> _userManager;
        public AppUserRepository(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<AppUser> deleteUserAsync(string userID)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userID);

            if(user == null)
            {
                return null;
            }

            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                throw new Exception("Failed to delete the user from the database.");
            }

            return user;
        }

        public async Task<List<AppUser>> GetAllUserAsync(AppUserQueryObject query)
        {
            var users = _userManager.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.FirstName))
            {
                users = users.Where(u => u.FirstName.Contains(query.FirstName));
            }

            if (!string.IsNullOrWhiteSpace(query.LastName))
            {
                users = users.Where(u => u.LastName.Contains(query.LastName));
            }

            var userList = await users.ToListAsync();

            return userList;
        }

        public async Task<AppUser> GetUserByIdAsync(string id)
        {
           var user = await _userManager.Users.FirstOrDefaultAsync(u =>u.Id == id);

            if(user == null)
            {
                return null;
            }

            return user;
        }

        public async Task<AppUser> UpdateUserAsync(string userID, UpdateUserDto updateUser)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userID);

            if(user == null)
            {
                return null;
            }

            user.ToUpdateUser(updateUser);

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                throw new Exception("Failed to update the user in the database.");
            }

            return user;
        }
    }
}
