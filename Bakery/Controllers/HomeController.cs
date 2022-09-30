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
        public IActionResult Catalog()
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

        [Authorize(Roles = Constants.RoleNames.Admin)]
        public IActionResult AddProduct()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = Constants.RoleNames.Admin)]
        public IActionResult AddProduct(Product product)
        {
            _context.Products.Add(product);
            _context.SaveChanges();

            return RedirectToAction("Catalog");
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

            return RedirectToAction("Catalog");
        }

        public IActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Registration(string login, string password, string name)
        {
            var existingUser = _context.Users.FirstOrDefault(u => u.Login == login);
            if (existingUser != null)
            {
                return Content($"Пользователь с таким логином уже существует.");
            }

            var newUser = new User(login, password, name, Tools.GetRoleId(_context, Constants.RoleNames.User));
            _context.Users.Add(newUser);
            _context.SaveChanges();

            return RedirectToAction("Login");
        }
    }
}