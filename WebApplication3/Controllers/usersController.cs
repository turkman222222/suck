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
            if (!ModelState.IsValid)
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
            ViewBag.UserCount = _context.user.Count();
            ViewBag.CarCount = _context.Carss.Count();
            ViewBag.OrderCount = _context.izbr.Count();
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
        // Создание пользователя (только для админа)
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["rol_id"] = new SelectList(_context.rol, "id_rol", "name_rol");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("id_user,user_name,mail,login,password,rol_id")] user user)
        {
            if (!ModelState.IsValid)
            {
                // Проверка уникальности логина
                if (await _context.user.AnyAsync(u => u.login == user.login))
                {
                    ModelState.AddModelError("login", "Этот логин уже занят");
                    ViewData["rol_id"] = new SelectList(_context.rol, "id_rol", "name_rol", user.rol_id);
                    return View(user);
                }

                // Проверка уникальности email
                if (await _context.user.AnyAsync(u => u.mail == user.mail))
                {
                    ModelState.AddModelError("mail", "Этот email уже зарегистрирован");
                    ViewData["rol_id"] = new SelectList(_context.rol, "id_rol", "name_rol", user.rol_id);
                    return View(user);
                }

                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["rol_id"] = new SelectList(_context.rol, "id_rol", "name_rol", user.rol_id);
            return View(user);
        }

        // Редактирование пользователя (только для админа)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.user.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            ViewData["rol_id"] = new SelectList(_context.rol, "id_rol", "name_rol", user.rol_id);
            return View(user);
        }

        

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("id_user,user_name,mail,login,password,rol_id")] user user)
        {
            if (id != user.id_user)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                try
                {
                    // Проверка уникальности логина
                    if (await _context.user.AnyAsync(u => u.login == user.login && u.id_user != user.id_user))
                    {
                        ModelState.AddModelError("login", "Этот логин уже занят");
                        ViewBag.rol_id = new SelectList(_context.rol, "id_rol", "name_rol", user.rol_id);
                        return View(user);
                    }

                    // Проверка уникальности email
                    if (await _context.user.AnyAsync(u => u.mail == user.mail && u.id_user != user.id_user))
                    {
                        ModelState.AddModelError("mail", "Этот email уже зарегистрирован");
                        ViewBag.rol_id = new SelectList(_context.rol, "id_rol", "name_rol", user.rol_id);
                        return View(user);
                    }

                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!userExists(user.id_user))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            // Повторно загружаем роли, если ModelState невалиден
            ViewBag.rol_id = new SelectList(_context.rol, "id_rol", "name_rol", user.rol_id);
            return View(user);
        }

        // Удаление пользователя (только для админа)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
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

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.user.FindAsync(id);
            if (user != null)
            {
                // Нельзя удалить самого себя
                var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                if (user.id_user == currentUserId)
                {
                    TempData["ErrorMessage"] = "Вы не можете удалить самого себя";
                    return RedirectToAction(nameof(Index));
                }

                _context.user.Remove(user);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool userExists(int id)
        {
            return _context.user.Any(e => e.id_user == id);
        }
    }
}