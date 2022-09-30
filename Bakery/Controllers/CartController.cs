using Bakery.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bakery.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        BakeryContext _context;

        public CartController(BakeryContext context)
        {
            _context = context;
        }

        public IActionResult ShowCart()
        {
            var userId = Tools.GetUserId(_context, User);
            
            return View
            (
                _context.CartItems.Include(i => i.Product)
                    .Where(i => i.UserId == userId)
                    .ToList()
            );
        }

        public IActionResult Add(int? productId)
        {
            if (productId is null)
            {
                return new NoContentResult();
            }

            var userId = Tools.GetUserId(_context, User);

            var item = _context.CartItems.FirstOrDefault(i => i.ProductId == productId && i.UserId == userId);
            if (item is null)
            {
                var product = _context.Products.FirstOrDefault(p => p.Id == productId);
                _context.CartItems.Add(new CartItem() { Count = 1, Product = product, UserId = userId });
            }
            else
            {
                item.Count++;
            }

            _context.SaveChanges();
            return new NoContentResult();
        }

        public IActionResult IncreaseCountByOne(int? cartItemId)
        {
            if (cartItemId == null)
            {
                return new NoContentResult();
            }

            var item = _context.CartItems.Where(i => i.Id == cartItemId).FirstOrDefault();
            if (item == null)
            {
                return new NoContentResult();
            }

            item.Count++;
            _context.SaveChanges();
            return RedirectToAction("ShowCart");
        }

        public IActionResult DecreaseCountByOne(int? cartItemId)
        {
            if (cartItemId == null)
            {
                return new NoContentResult();
            }

            var item = _context.CartItems.Where(i => i.Id == cartItemId).FirstOrDefault();
            if (item == null)
            {
                return new NoContentResult();
            }

            if (item.Count > 1)
            {
                item.Count--;
                _context.SaveChanges();

                return RedirectToAction("ShowCart");
            }
            else
            {
                return Delete(item.Id);
            }
        }

        public IActionResult Delete(int? cartItemId)
        {
            if (cartItemId == null)
            {
                return new NoContentResult();
            }

            var item = _context.CartItems.Where(i => i.Id == cartItemId).FirstOrDefault();
            if (item == null)
            {
                return new NoContentResult();
            }
            _context.Remove(item);
            _context.SaveChanges();
            return RedirectToAction("ShowCart");
        }

        public IActionResult Checkout()
        {
            var userId = Tools.GetUserId(_context, User);

            if (!_context.CartItems.Any(i => i.UserId == userId))
            {
                var errorMessage = "Невозможно оформить заказ: в Корзине нет товаров.";
                return View("../Home/Error", errorMessage);
            }

            var items = _context.CartItems.Include(i => i.Product).Where(i => i.UserId == userId).ToList();
            var totalSum = items.Sum(i => i.Product.Price * i.Count);
            var totalCount = items.Sum(i => i.Count);

            var newOrder = new Order()
            {
                Amount = totalSum,
                Date = DateTime.Now,
                UserId = userId
            };

            _context.Orders.Add(newOrder);
            _context.SaveChanges();

            foreach (var cartItem in items)
            {
                var productInOrder = new ProductInOrder() 
                { 
                    Order = newOrder,
                    Product = cartItem.Product,
                    Count = cartItem.Count
                };

                _context.ProductInOrders.Add(productInOrder);
                _context.Remove(cartItem);
            }

            _context.SaveChanges();
            return View("../Order/OrderCreated", (totalCount, totalSum));
        }
    }
}
