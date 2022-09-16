using Bakery.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;

namespace Bakery.Controllers
{
    public class HomeController : Controller
    {
        BakeryContext _context;

        public HomeController(BakeryContext context)
        {
            _context = context;
        }

        [Authorize]
        public IActionResult Index()
        {
            return View(_context.Products.ToList());
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string? returnUrl)
        {
            // получаем из формы email и пароль
            var form = Request.Form;
            // если email и/или пароль не установлены, посылаем статусный код ошибки 400
            if (!form.ContainsKey("login") || !form.ContainsKey("password"))
                return BadRequest("Логин и/или пароль не установлены");

            string login = form["login"];
            string password = form["password"];

            // находим пользователя 
            User? user = _context.Users.Include(u => u.Role).FirstOrDefault(u => u.Login == login && u.Password == password);
            // если пользователь не найден, отправляем статусный код 401
            if (user is null) return Unauthorized();

            var claims = new List<Claim> 
            { 
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Login),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role.Name)
            };
            // создаем объект ClaimsIdentity
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Cookies");
            // установка аутентификационных куки
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
            return Redirect(returnUrl ?? "/");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("/Home/Login");
        }

        public IActionResult AccessDenied()
        {
            return Content($"Access Denied");
        }
        
        [Authorize(Roles = "Admin")]
        public IActionResult AddProduct()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = Constants.RoleNames.Admin)]
        public async Task<IActionResult> AddProduct(Product product)
        {
            _context.Products.Add(product);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }


        [Authorize(Roles = Constants.RoleNames.Admin)]
        public IActionResult DeleteProduct(int? productId)
        {
            if (productId is null)
            {
                return new NoContentResult();
            }

            var product = _context.Products.FirstOrDefault(p => p.Id == productId);
            if (product != null)
            {
                _context.Remove(product);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
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