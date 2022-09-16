using Microsoft.EntityFrameworkCore;
using System.Numerics;

namespace Bakery.Models
{
    public class BakeryContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<ProductInOrder> ProductInOrders { get; set; }

        public BakeryContext(DbContextOptions<BakeryContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
