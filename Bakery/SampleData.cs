using Bakery.Models;
using System.Numerics;

namespace Bakery
{
    public static class SampleData
    {
        public static void Initialize(BakeryContext context)
        {
            if (!context.Products.Any())
            {
                context.Products.AddRange
                (
                    new Product
                    {
                        Name = "Французская булочка",
                        Price = 50
                    },
                    new Product
                    {
                        Name = "Тортик кремовый",
                        Price = 240
                    },
                    new Product
                    {
                        Name = "Тортик йогуртовый",
                        Price = 310
                    },
                    new Product
                    {
                        Name = "Сырок с начинкой \"Ваниль\"",
                        Price = 32
                    },
                    new Product
                    {
                        Name = "Сырок с начинкой \"Варёная сгущёнка\"",
                        Price = 31
                    },
                    new Product
                    {
                        Name = "Молочный (конечно же) шоколад",
                        Price = 60
                    },
                    new Product
                    {
                        Name = "Невкусный шоколад (чёрный)",
                        Price = 75
                    }
                );

                context.SaveChanges();
            }

            //context.RemoveRange(context.Roles);
            
            if (!context.Roles.Any())
            {
                context.Roles.AddRange
                (
                    new Role { Name = Constants.RoleNames.Admin },
                    new Role { Name = Constants.RoleNames.User }
                );

                context.SaveChanges();
            }

            if (!context.Users.Any())
            {
                var adminRoleId = context.Roles.FirstOrDefault(r => r.Name == Constants.RoleNames.Admin).Id;
                var userRoleId = context.Roles.FirstOrDefault(r => r.Name == Constants.RoleNames.User).Id;

                context.Users.AddRange
                (
                    new User("user1", "user1", null, userRoleId),
                    new User("user2", "user2", null, userRoleId),
                    new User("admin", "admin", null, adminRoleId)
                );

                context.SaveChanges();
            }
        }
    }
}
