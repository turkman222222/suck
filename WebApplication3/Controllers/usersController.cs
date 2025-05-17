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

        // Добавленные методы для авторизации/регистрации

        // GET: users/Login
        public IActionResult Login()
        {
            return View("~/Views/users/Login.cshtml");
        }
        [Authorize(Roles = "User")]
        public IActionResult UserDashboard()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        public IActionResult AdminDashboard()
        {
            return View();
        }

        // POST: users/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string login, string password)
        {
            var user = await _context.user
                .Include(u => u.rol)
                .FirstOrDefaultAsync(u => u.login == login && u.password == password);

            if (user == null)
            {
                ViewBag.Error = "Неверный логин или пароль";
                return View();
            }

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.user_name),
        new Claim(ClaimTypes.Email, user.mail),
        new Claim(ClaimTypes.NameIdentifier, user.id_user.ToString()),
        new Claim(ClaimTypes.Role, user.rol_id == 2 ? "Admin" : "User")
    };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            // Redirect based on role
            return user.rol_id == 2 ? RedirectToAction("AdminDashboard") : RedirectToAction("UserDashboard");
        }

        // GET: users/Register
        // GET: users/Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: users/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([Bind("user_name,mail,login,password")] user user)
        {
            if (ModelState.IsValid)
            {
                // Check for existing login
                if (await _context.user.AnyAsync(u => u.login == user.login))
                {
                    ModelState.AddModelError("login", "Пользователь с таким логином уже существует");
                    return View(user);
                }

                // Check for existing email
                if (await _context.user.AnyAsync(u => u.mail == user.mail))
                {
                    ModelState.AddModelError("mail", "Пользователь с таким email уже существует");
                    return View(user);
                }

                // Set default role (1 - regular user)
                user.rol_id = 1;

                _context.Add(user);
                await _context.SaveChangesAsync();

                // Automatic login after registration
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.user_name),
            new Claim(ClaimTypes.Email, user.mail),
            new Claim(ClaimTypes.NameIdentifier, user.id_user.ToString()),
            new Claim(ClaimTypes.Role, user.rol_id == 2 ? "Admin" : "User")
        };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                // Redirect based on role
                return user.rol_id == 2 ? RedirectToAction("AdminDashboard") : RedirectToAction("UserDashboard");
            }
            return View(user);
        }

        // GET: users/Logout
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction(nameof(Index));
        }

        // Существующие методы контроллера (не изменяем)

        // GET: users
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.user.Include(u => u.rol);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: users/Details/5
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

        // GET: users/Create
        public IActionResult Create()
        {
            ViewData["rol_id"] = new SelectList(_context.rol, "id", "id");
            return View();
        }

        // POST: users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id_user,user_name,mail,login,password,rol_id")] user user)
        {
            if (ModelState.IsValid)
            {
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["rol_id"] = new SelectList(_context.rol, "id", "id", user.rol_id);
            return View(user);
        }


        // GET: users
      
       

        // GET: users/Edit/5
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
            ViewData["rol_id"] = new SelectList(_context.rol, "id", "id", user.rol_id);
            return View(user);
        }

        // POST: users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id_user,user_name,mail,login,password,rol_id")] user user)
        {
            if (id != user.id_user)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
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
            ViewData["rol_id"] = new SelectList(_context.rol, "id", "id", user.rol_id);
            return View(user);
        }

        // GET: users/Delete/5
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

        // POST: users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.user.FindAsync(id);
            if (user != null)
            {
                _context.user.Remove(user);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool userExists(int id)
        {
            return _context.user.Any(e => e.id_user == id);
        }
    }
}
