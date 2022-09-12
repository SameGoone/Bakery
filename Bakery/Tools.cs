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
                throw new Exception("Пользователь не найден");
            }

            return userEntity.Id;
        }
    }
}
