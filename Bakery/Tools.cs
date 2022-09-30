using Bakery.Models;
using System.Security.Claims;

namespace Bakery
{
    public class Tools
    {
        public static int GetUserId(BakeryContext context, ClaimsPrincipal user)
        {
            var userName = user.Identity.Name;
            var userEntity = context.Users.FirstOrDefault(u => u.Login == userName);

            if (userEntity == null)
            {
                throw new Exception("Пользователь не найден.");
            }

            return userEntity.Id;
        }

        public static int GetRoleId(BakeryContext context, string roleName)
        {
            var role = context.Roles.FirstOrDefault(r => r.Name == roleName);
            if (role == null)
            {
                throw new Exception($"Роль с именем {roleName} не найдена.");
            }

            return role.Id;
        }
    }
}
