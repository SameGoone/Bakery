using Bakery.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bakery.Controllers
{
    public class OrderController : Controller
    {
        BakeryContext _context;

        public OrderController(BakeryContext context)
        {
            _context = context;
        }

        public IActionResult ShowOrders()
        {
            var userId = Tools.GetUserId(_context, User);

            return View
            (
                _context.Orders
                    .Where(o => o.UserId == userId)
                    .OrderByDescending(o => o.Date)
                    .ToList()
            );
        }

        public IActionResult ShowProductInOrders(int? orderId)
        {
            return View
            (
                _context.ProductInOrders
                    .Include(p => p.Product)
                    .Include(p => p.Order)
                    .Where(p => p.OrderId == orderId)
                    .ToList()
            );
        }
    }
}
