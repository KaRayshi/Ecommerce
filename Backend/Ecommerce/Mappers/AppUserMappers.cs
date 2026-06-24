using Ecommerce.Dto.User;
using Ecommerce.Models;

namespace Ecommerce.Mappers
{
    public static class AppUserMappers
    {
        public static AppUserDto ToAppUserDto(this AppUser user)
        {
            return new AppUserDto
            {
                AppUserId = user.Id,
                Username = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email
            };
        }

        public static void ToUpdateUser(this AppUser userName, UpdateUserDto updateUser)
        {
            if (!string.IsNullOrWhiteSpace(updateUser.Username))
            {
                userName.UserName = updateUser.Username;
            }

            if (!string.IsNullOrWhiteSpace(updateUser.FirstName))
            {
                userName.FirstName = updateUser.FirstName;
            }

            if (!string.IsNullOrWhiteSpace(updateUser.LastName))
            {
                userName.LastName = updateUser.LastName;
            }
        }
    }
}
