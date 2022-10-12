using Bakery.Core.Interfaces;
using Bakery.Infrastructure;
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
        //BakeryContext _context;
        public readonly IProductRepository _productRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;

        public HomeController(IProductRepository productRepository, IUserRepository userRepository, IRoleRepository roleRepository)
        {
            _productRepository = productRepository;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
        }

        //public HomeController(BakeryContext context)
        //{
        //    _context = context;
        //}

        [Authorize]
        public async Task<IActionResult> CatalogAsync()
        {
            return View(await _productRepository.ListAsync());
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
            User? user = _userRepository.ListAsync().Result.FirstOrDefault(u => u.Login == login && u.Password == password);
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
            var res = Content($"Access Denied");
            return Content($"Access Denied");
        }

        [Authorize(Roles = Constants.RoleNames.Admin)]
        public IActionResult AddProduct()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = Constants.RoleNames.Admin)]
        public async Task<IActionResult> AddProductAsync(Product product)
        {
            await _productRepository.AddAsync(product);

            return RedirectToAction("Catalog");
        }


        [Authorize(Roles = Constants.RoleNames.Admin)]
        public async Task<IActionResult> DeleteProductAsync(int? productId)
        {
            if (productId is null)
            {
                return new NoContentResult();
            }

            var product = await _productRepository.GetByIdAsync((int)productId);
            if (product != null)
            {
                await _productRepository.RemoveAsync(product);
            }

            return RedirectToAction("Catalog");
        }

        public IActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegistrationAsync(string login, string password, string name)
        {
            var existingUser = _userRepository.ListAsync().Result.FirstOrDefault(u => u.Login == login);
            if (existingUser != null)
            {
                return Content($"Пользователь с таким логином уже существует.");
            }

            var newUser = new User(login, password, name, Tools.GetRoleId(_roleRepository, Constants.RoleNames.User));
            await _userRepository.AddAsync(newUser);

            return RedirectToAction("Login");
        }
    }
}