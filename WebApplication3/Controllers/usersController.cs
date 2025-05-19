using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication3.Models;
using Microsoft.AspNetCore.Authorization;

namespace WebApplication3.Controllers
{
    public class usersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public usersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Страница входа (доступна без авторизации)
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        // Обработка входа (POST)
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string login, string password)
        {
            // Ищем пользователя по логину и паролю (без хэширования)
            var user = await _context.user
                .Include(u => u.rol)
                .FirstOrDefaultAsync(u => u.login == login && u.password == password);

            if (user == null)
            {
                ViewBag.Error = "Неверный логин или пароль";
                return View();
            }

            // Создаем claims для аутентификации
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.user_name),
                new Claim(ClaimTypes.Email, user.mail),
                new Claim(ClaimTypes.NameIdentifier, user.id_user.ToString()),
                new Claim(ClaimTypes.Role, user.rol_id == 2 ? "Admin" : "User")
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            // Входим в систему
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            // Перенаправляем в зависимости от роли
            return RedirectToAction(user.rol_id == 2 ? "AdminDashboard" : "UserDashboard");
        }

        // Страница регистрации
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        // Обработка регистрации (POST)
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([Bind("user_name,mail,login,password")] user user)
        {
            if (ModelState.IsValid)
            {
                // Проверяем уникальность логина
                if (await _context.user.AnyAsync(u => u.login == user.login))
                {
                    ModelState.AddModelError("login", "Этот логин уже занят");
                    return View(user);
                }

                // Проверяем уникальность email
                if (await _context.user.AnyAsync(u => u.mail == user.mail))
                {
                    ModelState.AddModelError("mail", "Этот email уже зарегистрирован");
                    return View(user);
                }

                // Устанавливаем роль "Пользователь" (1) по умолчанию
                user.rol_id = 1;

                // Сохраняем пользователя (пароль в открытом виде)
                _context.Add(user);
                await _context.SaveChangesAsync();

                // Автоматически входим после регистрации
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.user_name),
                    new Claim(ClaimTypes.Email, user.mail),
                    new Claim(ClaimTypes.NameIdentifier, user.id_user.ToString()),
                    new Claim(ClaimTypes.Role, "User") // Все новые пользователи - обычные
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                return RedirectToAction("UserDashboard");
            }
            return View(user);
        }

        // Выход из системы
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login");
        }

        // Личный кабинет пользователя
        [Authorize(Roles = "User")]
        public IActionResult UserDashboard()
        {
            return View();
        }

        // Панель администратора
        [Authorize(Roles = "Admin")]
        public IActionResult AdminDashboard()
        {
            return View();
        }

        // Список всех пользователей (только для админа)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var users = await _context.user.Include(u => u.rol).ToListAsync();
            return View(users);
        }

        // Просмотр профиля пользователя
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.user
                .Include(u => u.rol)
                .FirstOrDefaultAsync(m => m.id_user == id);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // Остальные методы (Create, Edit, Delete) аналогично, 
        // но защищены атрибутами [Authorize]
    }
}